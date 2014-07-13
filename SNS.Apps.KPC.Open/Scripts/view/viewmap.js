/*
*   Detail map js
*/
function openBaiduMap(route_self, route_fellow) {
    var MapOptions = {
        enableHighResolution: true
    };
    var map = new BMap.Map("map_canvas", MapOptions);
    var point = new BMap.Point(116.404, 39.915);
    map.enablePinchToZoom();
    map.enableDragging();
    map.enableInertialDragging();
    map.enableAutoResize();
    map.highResolutionEnabled();

    // map.addControl
    var opts = {
        offset: new BMap.Size(10, 20)
    };

    map.addControl(new BMap.ScaleControl(opts));	// left corner
    map.addControl(new BMap.NavigationControl()); //right corner

    map.centerAndZoom(point, 6);

    var bounds = null;
    var linesPoints = null;
    var spoi1 = new BMap.Point(route_self.frompoint.lng, route_self.frompoint.lat);    // 起点1
    var spoi2 = new BMap.Point(route_fellow.frompoint.lng, route_fellow.frompoint.lat);    // 起点2
    var epoi1 = new BMap.Point(route_self.topoint.lng, route_self.topoint.lat);    // 终点
    var epoi2 = new BMap.Point(route_fellow.topoint.lng, route_fellow.topoint.lat);
    var myIcon = new BMap.Icon("Mario.png", new BMap.Size(32, 70), { imageOffset: new BMap.Size(0, 0) });
    initLine();

    function initLine() {
        bounds = new Array();
        linesPoints = new Array();
        map.clearOverlays();                                                    // 清空覆盖物
        var driving3 = new BMap.DrivingRoute(map, {
            renderOptions: { map: map, autoViewport: true }, onMarkersSet: function (pois) {
                //pois[0].marker.setIcon(new BMap.Icon("/content/images/kpc_detail_start.png", new BMap.Size(30, 30)));
                //pois[1].marker.setIcon(new BMap.Icon("/content/images/kpc_detail_end.png", new BMap.Size(30, 30)));
            }
        });  // 驾车实例,并设置回调
        driving3.search(spoi1, epoi1);                                       // 搜索一条线路
        var driving4 = new BMap.DrivingRoute(map, { renderOptions: { map: map, autoViewport: true } });  // 驾车实例,并设置回调
        driving4.search(spoi2, epoi2);                          // 搜索一条线路
        driving3.enableAutoViewport();
        driving4.enableAutoViewport();
    }
}
