//window.onload = initialize;
function initialize(flag) {
    var city_location = window.parent.globals_currentcity + window.parent.globals;
    // map.centerAndZoom
    var mapoption = {
        enableHighResolution: true,
    };

    var map = new BMap.Map("map_canvas", mapoption);
    var point = new BMap.Point(121.491, 31.233);
    map.enablePinchToZoom();
    map.enableDragging();
    map.enableInertialDragging();
    map.enableAutoResize();
    map.highResolutionEnabled();

    // map.addControl
    var opts = {
        offset: new BMap.Size(10, 20)
    };

    var opts2 = {
        offset: new BMap.Size(20, 20)
    };

    var opts3 = {
        anchor: BMAP_ANCHOR_TOP_LEFT
    };


    map.addControl(new BMap.ScaleControl(opts));	// left corner
    //map.addControl(new BMap.GeolocationControl(opts2)); // middle
    map.addControl(new BMap.NavigationControl()); //right corner

    // map.addOverlay
    var myGeo = new BMap.Geocoder();
    var marker = new BMap.Marker();

    if (window.parent.coordinate) {
        point = new BMap.Point(window.parent.coordinate.longitude, window.parent.coordinate.latitude);

        map.centerAndZoom(point, 18);//16
        marker = new BMap.Marker(point);
        map.addOverlay(marker);
        document.getElementById("div_location").innerHTML = city_location;
    }
    else
        // getPoint priority input ? "input" : "cityCode"
        if (city_location !== null && city_location !== "" && city_location !== undefined) {

            map.removeOverlay(marker);

            myGeo.getPoint(city_location, function (point) {
                if (point) {
                    marker = new BMap.Marker(point);
                    if (window.parent.globals !== "") {
                        map.centerAndZoom(point, 18);
                        map.addOverlay(marker);
                        document.getElementById("div_location").innerHTML = city_location;
                    }
                    else {
                        map.centerAndZoom(point, 14);
                        document.getElementById("div_location").innerHTML = "请在地图上点选";
                    }
                } else {
                    map.centerAndZoom("上海", 14);
                    document.getElementById("div_location").innerHTML = "请在地图上点选";
                }
            });
        } else if (window.parent.locationPoint) {
            point = new BMap.Point(window.parent.locationPoint.longitude, window.parent.locationPoint.latitude);
            if (flag)
                map.centerAndZoom(point, 16);//16
            else
                map.centerAndZoom(point, 18);//16
            marker = new BMap.Marker(point);
            map.addOverlay(marker);

            // getLoaction
            myGeo.getLocation(point, function (result) {
                if (result) {
                    var add = result.addressComponents;
                    window.parent.globals_currentcity = add.city;
                    window.parent.globals = add.district + add.street + add.streetNumber;
                    //document.getElementById("div_location").innerHTML = result.address;
                    document.getElementById("div_location").innerHTML = add.city + add.district + add.street + add.streetNumber;
                    //window.parent.globals = add.city + add.district + add.street + add.streetNumber;
                }
            });

        } else {
            //map.centerAndZoom(window.parent.cityCode, 12);
            document.getElementById("div_location").innerHTML = "请在地图上点选";
            if (window.parent.latestAddress == null) {
                map.centerAndZoom("上海", 14);
            } else {
                point = new BMap.Point(window.parent.latestAddress.lng, window.parent.latestAddress.lat);
                map.centerAndZoom(point, 14);
            }
        }

    /* click事件 */
    map.addEventListener("click", function (e) {
        point = new BMap.Point(e.point.lng, e.point.lat);
        // add 2013/09/07
        window.parent.coordinate = {
            "longitude": e.point.lng,
            "latitude": e.point.lat
        };

        // map.addOverlay
        this.removeOverlay(marker);

        marker = new BMap.Marker(point);
        this.addOverlay(marker);
        document.getElementById("div_location").innerHTML = "地址正在解析中";
        // getLoaction
        myGeo.getLocation(point, function (result) {
            if (result) {
                var add = result.addressComponents;
                window.parent.globals_currentcity = add.city;
                window.parent.globals = add.district + add.street + add.streetNumber;
                //document.getElementById("div_location").innerHTML = result.address;
                document.getElementById("div_location").innerHTML = add.city + add.district + add.street + add.streetNumber;
                //window.parent.globals = add.city + add.district + add.street + add.streetNumber;
            }
            else {
                document.getElementById("div_location").innerHTML = "地址解析失败";
            }
        });

        //window.parent.locationPoint = e.point;
    });

}
console.log("end2");