var evt = document.createEvent("MouseEvents");
//evt.initMouseEvent("click", true, false);
evt.initEvent("click", false, false);
var loadLocation = false;
var placeholderStrBx = "请输入地址，如小区(选填)", placeholderStrX = "请输入详细地址(选填)", placeholderStrBy = "请输入地址，如公司(选填)";
var typeselected = "lbltongchengall", dateselected = "lbldateall", roleselected = "lblall";
document.getElementById("sxb").addEventListener("click", function () {
    this.className = "";
    var data1 = [["ct", "className", "active"],
                ["sxb_date", "className", "search"],
                ["ct_date", "className", "search ct"],
                ["radiosxb", "checked", "checked"],
                ["destination", ["style", "display"], "none"],
				["from_map", "placeholder", placeholderStrBx],
				["to_map", "placeholder", placeholderStrBy],
                ["tongchengall", "checked", "checked"]];

    freshStyle(data1);
    var now_Expires = new Date();
    now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
    document.cookie = "searchpintype=sxb;path=/;expires=" + now_Expires.toGMTString();
    LoadCityandLocation();
    document.getElementById("tocity").value = document.getElementById("fromcity").value;
    document.getElementById(typeselected).className = "";
    typeselected = "lbltongchengall";
    document.getElementById(typeselected).className = "selected";
});

function addEvent() {
    document.getElementById("lbltongchengall").addEventListener("click", noneStyle);
    document.getElementById("lblworkday").addEventListener("click", noneStyle);
    document.getElementById("lbltwoday").addEventListener("click", noneStyle);
    document.getElementById("lbltempday").addEventListener("click", showStyle);
}

function noneStyle() {
    document.getElementById("ct_date").className = "search ct";
    if (this.id !== typeselected) {
        document.getElementById(this.id).className = "selected";
        document.getElementById(typeselected).className = "";
        typeselected = this.id;
    }
}

function showStyle() {
    document.getElementById("ct_date").className = "search";
    if (this.id !== typeselected) {
        document.getElementById(this.id).className = "selected";
        document.getElementById(typeselected).className = "";
        typeselected = this.id;
    }
}

function freshStyle(data) {
    for (var i = 0, len = data.length; i < len; i++) {
        if (data[i][1] instanceof Array) {
            document.getElementById(data[i][0])[data[i][1][0]][data[i][1][1]] = data[i][2];
        } else {
            document.getElementById(data[i][0])[data[i][1]] = data[i][2];
        }
    }
}

document.getElementById("ct").addEventListener("click", function () {
    this.className = "";
    var data2 = [["sxb", "className", "active"],
        ["sxb_date", "className", "search sxb"],
        ["ct_date", "className", "search ct"],
        ["radioct", "checked", "checked"],
        ["destination", ["style", "display"], "block"],
		["from_map", "placeholder", placeholderStrX],
        ["to_map", "placeholder", placeholderStrX],
        ["tongchengall", "checked", "checked"]];
    freshStyle(data2);
    var now_Expires = new Date();
    now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
    document.cookie = "searchpintype=ct;path=/;expires=" + now_Expires.toGMTString();
    LoadCityandLocation();
    document.getElementById(typeselected).className = "";
    typeselected = "lbltongchengall";
    document.getElementById(typeselected).className = "selected";
});

addEvent();

/*
var arr = ["search", "create", "searchLocation", "createLocation", "searchpintype", "createpintype", "cityhistory"];
for (var i = 0; i < arr.length; i++) {
    clearCookie(arr[i]);
}
function clearCookie(clearstr) {
    var cityhistory_expires = clearstr + "=;expires=Sat, 01-Jan-2000 00:00:00 GMT";
    //document.cookie = cityhistory_expires;
    document.cookie = cityhistory_expires + ";path=/route/;";
    document.cookie = cityhistory_expires + ";domain=kuaipinche.com;";
    //document.cookie = cityhistory_expires + ";domain=open.kuaipinche.com;";
    document.cookie = cityhistory_expires + ";path=/route/;domain=kuaipinche.com;";
    document.cookie = cityhistory_expires + ";path=/route/;domain=open.kuaipinche.com;";
    document.cookie = cityhistory_expires + ";path=/;domain=kuaipinche.com;";
    //document.cookie = cityhistory_expires + ";path=/;domain=open.kuaipinche.com;";
}
*/

setSearchType();

/* 设置区域 始发地/目的地 城市 */
//LoadCityandLocation();

function returnCookie(searchstr) {
    var str = null;
    if (document.cookie.length > 0) {
        var numstart = document.cookie.indexOf(searchstr + "="), numend = -1;
        if (numstart != -1) {
            numstart = numstart + searchstr.length + 1;
            numend = document.cookie.indexOf(";", numstart);
            if (numend !== -1)
                str = document.cookie.substring(numstart, numend);
            else
                str = document.cookie.substring(numstart);
        }
    }
    return str;
}

function LoadCityandLocation() {
    var cityhistory = null;
    cityhistory = returnCookie("cityhistory"),
    searchpintype = returnCookie("searchpintype"),
    whichtype = "city_" + searchpintype;
    var temp = { fromcity: cityhistory === null ? "" : cityhistory, tocity: searchpintype === null ? (cityhistory === null ? "" : cityhistory) : "" };
    var cookiesearch = returnCookie(whichtype);
    if (!JSON.parse(cookiesearch)) {
        //var now_Expires = new Date();
        //now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
        document.cookie = whichtype + "={\"fromcity\":\"" + temp.fromcity + "\",\"tocity\":\"" + temp.tocity + "\"};path=/;";
    }
    var sessionCity = JSON.parse(returnCookie(whichtype));
    if (sessionCity.fromcity === "") {
        loadLocation = true;
    }
    document.getElementById("fromcity").value = unescape(sessionCity.fromcity);
    document.getElementById("tocity").value = unescape(sessionCity.tocity);
    var sessionLocation = (returnCookie("Location_" + searchpintype) === null ? null : JSON.parse(returnCookie("Location_" + searchpintype))) || { fromlocation: "", tolocation: "" };
    document.getElementById("from_map").value = unescape(sessionLocation.fromlocation);
    document.getElementById("to_map").value = unescape(sessionLocation.tolocation);

    // set coordTo to null important!
    coordFrom = null;
    coordTo = null;
}


function setSearchType() {
    var searchpintype = returnCookie("searchpintype");
    if (searchpintype) {
        if (document.all) {
            document.getElementById(searchpintype).click();
        } else {
            document.getElementById(searchpintype).dispatchEvent(evt);
        }
    } else {
        if (document.all) {
            document.getElementById("sxb").click();
        } else {
            document.getElementById("sxb").dispatchEvent(evt);
        }
    }
}


function saveLocation(e) {
    //window.sessionStorage.removeItem("searchLocation");
    var location = {
        fromlocation: escape(document.getElementById("from_map").value),
        tolocation: escape(document.getElementById("to_map").value)
    };
    //location[e] = null;
    if (arguments.length !== 0) {
        location[e] = "";
    }
    var whichtype = "Location_" + returnCookie("searchpintype");
    //window.sessionStorage.setItem("searchLocation", JSON.stringify(location));
    var now_Expires = new Date();
    now_Expires.setTime(now_Expires.getTime() + 30 * 24 * 60 * 60 * 1000);
    document.cookie = whichtype + "={\"fromlocation\":\"" + location.fromlocation + "\",\"tolocation\":\"" + location.tolocation + "\"};path=/;expires=" + now_Expires.toGMTString();
}

/* 始发地 城市 */
document.getElementById("fromcity_div").addEventListener("click", function () {
    showWait();
    saveLocation("fromlocation");
    window.location.href = window.location.protocol + "//" + window.location.host + "/route/city?citytype=fromcity&referrer=search";
});

/* 目的地 城市 */
document.getElementById("tocity_div").addEventListener("click", function () {
    showWait();
    saveLocation("tolocation");
    window.location.href = window.location.protocol + "//" + window.location.host + "/route/city?citytype=tocity&referrer=search";
});

/* 导航处理 */
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
