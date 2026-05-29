#nullable enable

using UIView = Android.Views.View;
using ViewGroup = Android.Views.ViewGroup;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Handlers;

namespace Maui.MapLibre.Handlers;

public partial class MapLibreMapHandler : ViewHandler<MapLibreMap, UIView>    
{
    private MapLibreMapController _controller = null!;
    private string _styleUrl = null!;
    private ViewTreeObserver.IOnGlobalLayoutListener? _layoutListener;
    
    public IMapLibreMapController Controller => _controller;
    
    public MapLibreMapHandler() : base(PropertyMapper) { }

    protected override void ConnectHandler(UIView platformView)
    {
        base.ConnectHandler(platformView);
        VirtualView.SizeChanged += OnVirtualViewSizeChanged;
        AttachLayoutListener(platformView);
        RequestViewportSync();
    }

    protected override void DisconnectHandler(UIView platformView)
    {
        VirtualView.SizeChanged -= OnVirtualViewSizeChanged;
        DetachLayoutListener(platformView);
        base.DisconnectHandler(platformView);
    }

    public void RequestViewportSync() => _controller?.SyncViewportLayout();

    private void OnVirtualViewSizeChanged(object? sender, System.EventArgs e) => RequestViewportSync();

    private void AttachLayoutListener(UIView platformView)
    {
        DetachLayoutListener(platformView);
        var observer = platformView.ViewTreeObserver;
        if (observer == null || !observer.IsAlive)
            return;

        _layoutListener = new GlobalLayoutListener(RequestViewportSync);
        observer.AddOnGlobalLayoutListener(_layoutListener);
    }

    private void DetachLayoutListener(UIView platformView)
    {
        if (_layoutListener == null)
            return;

        var observer = platformView.ViewTreeObserver;
        if (observer != null && observer.IsAlive)
            observer.RemoveOnGlobalLayoutListener(_layoutListener);

        _layoutListener = null;
    }

    private sealed class GlobalLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly Action _onLayout;

        public GlobalLayoutListener(Action onLayout) => _onLayout = onLayout;

        public void OnGlobalLayout() => _onLayout();
    }

    protected override UIView CreatePlatformView()
    {
        if (Platform.CurrentActivity == null)
        {
            throw new InvalidOperationException("MapLibreMapHandler requires a current activity.");
        }
        
        var activity = Platform.CurrentActivity;
        var context = activity.ApplicationContext;
        var options = new Dictionary<string, object> { { "styleString", _styleUrl } };
        
        if (context == null) throw new InvalidOperationException("MapLibreMapHandler requires a valid context.");

        _controller = MapLibreMapFactory.Create(activity, context, options);

        _controller.OnMapReadyReceived += VirtualView.OnMapReady;
        _controller.OnStyleLoadedReceived += VirtualView.OnStyleLoaded;
        _controller.OnDidBecomeIdleReceived += VirtualView.OnDidBecomeIdle;
        _controller.OnCameraMoveStartedReceived += VirtualView.OnCameraMoveStarted;
        _controller.OnCameraMoveReceived += VirtualView.OnCameraMove;
        _controller.OnCameraIdleReceived += VirtualView.OnCameraIdle;
        _controller.OnCameraTrackingChangedReceived += VirtualView.OnCameraTrackingChanged;
        _controller.OnCameraTrackingDismissedReceived += VirtualView.OnCameraTrackingDismissed;
        _controller.OnMapClickReceived += VirtualView.OnMapClick;
        _controller.OnMapLongClickReceived += VirtualView.OnMapLongClick;
        _controller.OnUserLocationUpdateReceived += VirtualView.OnUserLocationUpdate;
        
        // Init and then return the map view
        _controller.Init();
        var mapView = _controller.View;
        
        // Create the root layout
        var layout = new FrameLayout(activity);
        layout.LayoutParameters ??= new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        layout.LayoutParameters.Width = ViewGroup.LayoutParams.MatchParent;
        layout.LayoutParameters.Height = ViewGroup.LayoutParams.MatchParent;
        
        mapView.LayoutParameters = new FrameLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.MatchParent);

        layout.AddView(mapView);
        
        return layout;
    }
    
    public void UpdateStyleUrl(string styleUrl)
    {
        _styleUrl = styleUrl;
        _controller.SetStyleString(styleUrl);
    }
    
    public void UpdateMinMaxZoomPreference(double? minZoom, double? maxZoom)
    {
        _controller.SetMinMaxZoomPreference(minZoom, maxZoom);
    }
    
    public void UpdateRotateGestureEnabled(bool rotateGestureEnabled)
    {
        _controller.SetRotateGesturesEnabled(rotateGestureEnabled);
    }
    
    public void UpdateScrollGesturesEnabled(bool scrollGesturesEnabled)
    {
        _controller.SetScrollGesturesEnabled(scrollGesturesEnabled);
    }
    
    public void UpdateTiltGesturesEnabled(bool tiltGesturesEnabled)
    {
        _controller.SetTiltGesturesEnabled(tiltGesturesEnabled);
    }
    
    public void UpdateTrackCameraPosition(bool trackCameraPosition)
    {
        _controller.SetTrackCameraPosition(trackCameraPosition);
    }
    
    public void UpdateZoomGesturesEnabled(bool zoomGesturesEnabled)
    {
        _controller.SetZoomGesturesEnabled(zoomGesturesEnabled);
    }
    
    public void UpdateMyLocationEnabled(bool myLocationEnabled)
    {
        _controller.SetMyLocationEnabled(myLocationEnabled);
    }
    
    public void UpdateMyLocationTrackingMode(int myLocationTrackingMode)
    {
        _controller.SetMyLocationTrackingMode(myLocationTrackingMode);
    }
    
    public void UpdateMyLocationRenderMode(int myLocationRenderMode)
    {
        _controller.SetMyLocationRenderMode(myLocationRenderMode);
    }

    public void UpdateLogoViewMargins(int?[]? margin)
    {
        if (margin == null) return;
        var x = margin[0];
        var y = margin[1];
        if (x == null || y == null) return;
        _controller.SetLogoViewMargins((int) x, (int) y);
    }
    
    public void UpdateCompassGravity(int compassGravity)
    {
        _controller.SetCompassGravity(compassGravity);
    }
    
    public void UpdateCompassViewMargins(int?[]? margin)
    {
        if (margin == null) return;
        var x = margin[0];
        var y = margin[1];
        if (x == null || y == null) return;
        _controller.SetCompassViewMargins((int)x, (int)y);
    }
    
    public void UpdateAttributionButtonGravity(int attributionButtonGravity)
    {
        _controller.SetAttributionButtonGravity(attributionButtonGravity);
    }
    
    public void UpdateAttributionButtonMargins(int?[]? margin)
    {
        if (margin == null) return;
        var x = margin[0];
        var y = margin[1];
        if (x == null || y == null) return;
        _controller.SetAttributionButtonMargins((int)x, (int)y);
    }
}
