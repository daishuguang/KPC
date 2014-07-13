(function () {
    var num = 0, IsSendRequest = true, hashNum = 0, flag = false;
    document.addEventListener("DOMContentLoaded", function () {
        if (location.hash) {
            flag = true;
            hashNum = location.hash.substr(1);
            moreOrderList();
        }
    });

    window.addEventListener("scroll", function () {
        // 加载更多
        var documentscrolltop = document.body.scrollTop || document.documentElement.scrollTop;
        if ((window.innerHeight + documentscrolltop) >= document.body.scrollHeight - 3) {
            moreOrderList();
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

    function moreOrderList() {
        num++;
        var data_send = "city=" + _city + "&pagenum=" + num;

        if (IsSendRequest) {
            location.hash = num;
            document.getElementById("shade").style.display = "block";
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

                        console.log(flag);
                        console.log("hashNum" + hashNum);
                        console.log("num" + num);
                        // hide
                        document.getElementById("shade").style.display = "none";
                        if (flag && hashNum > num) {
                            moreOrderList();
                        }
                    }
                };

                var url = "/api/plazamore?";

                xmlhttp.open("GET", url + data_send, true);
                xmlhttp.send(null);
            }
        }
    }
})();