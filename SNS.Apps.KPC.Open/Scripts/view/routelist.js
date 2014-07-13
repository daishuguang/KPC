(function () {
    var num = 0, IsSendRequest = true;
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
        var data_send = "pageNum=" + num;

        if (IsSendRequest) {
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

                        // hide
                        document.getElementById("shade").style.display = "none";
                    }
                };

                var url = "/route/listmore";

                xmlhttp.open("POST", url, true);
                xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                xmlhttp.send(encodeURI(data_send));
            }
        }
    }
})();