(function () {
    var num = 0,
        IsSendRequest = true,
        role = window.location.search.substr(window.location.search.indexOf("=") + 1),
        hashNum = 0, flag = false;

    document.addEventListener("DOMContentLoaded", function () {
        if (location.hash) {
            flag = true;
            hashNum = location.hash.substr(1);
            pullUpAction();
        }
    });

    function pullUpAction() {
        num++;
        var data_send = {
            pagenum: num,
            role: role
        };

        if (IsSendRequest) {
            document.getElementById("shade").style.display = "block";
            location.hash = num;
            var xmlhttp;
            if (window.XMLHttpRequest) {
                xmlhttp = new XMLHttpRequest();
                xmlhttp.onreadystatechange = function () {
                    if (xmlhttp.readyState === 4 && xmlhttp.status === 200) {
                        var data = JSON.parse(xmlhttp.responseText);

                        if (data.flag) {
                            var template_script = document.getElementById("routelist_template").innerHTML;
                            var template_HTML = _.template(template_script, { datas: data.dataList });

                            document.getElementById("searchresult").innerHTML += template_HTML;
                        } else {
                            IsSendRequest = false;
                        }

                        // hide
                        document.getElementById("shade").style.display = "none";

                        console.log(flag);
                        console.log("hashNum" + hashNum);
                        console.log("num" + num);
                        if (flag && hashNum > num) {
                            pullUpAction();
                        }
                    }
                };

                xmlhttp.open("GET", "/api/aroundlistmore?pagenum=" + data_send.pagenum + "&role=" + data_send.role, true);
                xmlhttp.send(null);
            }
        }
    }

    //document.addEventListener('touchmove', function (e) { e.preventDefault(); }, false);

    //document.addEventListener('DOMContentLoaded', loaded, false);

    window.nav = function (param) {
        document.getElementById("navaroundlist").className = "navtoggle";
        if (IsShowWait) { showWait(); } else { WeixinJSBridge.call('closeWindow'); }
        window.location.href = window.location.protocol + "//" + window.location.host + window.location.pathname + "?role=" + window.event.target.getAttribute("data-value");
    }

    document.getElementById("category").addEventListener("click", function () {
        var element = document.getElementById("navaroundlist");
        if (element.className.match("navtoggle")) {
            element.className = "";
        } else
            element.className = "navtoggle";
    });

    document.getElementById('category').innerText = (role === "driver" ? "车主" : (role === "passenger" ? "乘客" : "全部"));

    window.addEventListener("scroll", function () {
        // 加载更多
        var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
        if ((window.innerHeight + documentscrolltop) >= document.body.scrollHeight - 3) {
            pullUpAction();
        }

        // back to top
        if (documentscrolltop > window.innerHeight) {
            document.getElementById("top_back").style.display = "block";
        } else {
            document.getElementById("top_back").style.display = "none";
        }
    });

    document.getElementById("navbar").addEventListener("click", function (e) {
        if (document.documentElement.style.position !== "fixed") {
            document.getElementById("nav").style.display = "block";
            document.getElementById("wrapper").className = "nav";
            document.documentElement.style.position = "fixed";
        } else {
            document.body.click();
        }
        e.stopPropagation();
    });

    document.body.addEventListener("click", function () {
        if (document.getElementById("nav").style.display === "block") {
            document.getElementById("nav").style.display = "none";
            document.getElementById("wrapper").className = "";
            document.documentElement.style.position = "";
        }
    });
})();