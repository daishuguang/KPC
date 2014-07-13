(function () {
    document.getElementById("backbtn").addEventListener("click", function () {
        if (refernull) {
            if (message) {
                WeixinJSBridge.call("closeWindow");
            } else {
                if (navigator.userAgent.indexOf("MicroMessenger/5.3") === -1) {
                    WeixinJSBridge.call("closeWindow");
                }
                window.location.href = "/route/plaza";
            }
        } else {
            history.back();
        }
    });

    var flag = false;
    document.getElementById("maptitle").addEventListener("click", function () {
        if (document.getElementById("map_canvas").style.display === "none") {
            document.getElementById("map_canvas").style.display = "block";
            document.getElementById("uparrow").style.display = "block";
            document.getElementById("downarrow").style.display = "none";
            if (!flag) {
                openBaiduMap(KPC.Peek.route_self, KPC.Peek.route_fellow);
                flag = true;
            }
        } else {
            document.getElementById("map_canvas").style.display = "none";
            document.getElementById("uparrow").style.display = "none";
            document.getElementById("downarrow").style.display = "block";
        }
    });
})();
