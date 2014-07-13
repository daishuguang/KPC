var evt = document.createEvent("MouseEvents");
//evt.initMouseEvent("click", true, false);
evt.initEvent("click", false, false);
var loadLocation = false;
var placeholderStrBx = "请输入地址，如小区(必填)", placeholderStrX = "请输入详细地址(选填)", placeholderStrBy = "请输入地址，如公司(必填)";

var tongcheng = [], callback;
var types = ["typeworkday", "typetwoday", "typetempday", "typeeachday"];
var typeselect = "typeworkday";
document.getElementById("sxb").addEventListener("click", function () {
    this.className = "";
    var eledata = [["ct", "className", "active"],
        ["sxb_date", "className", "search"],
        ["ct_date", "className", "search ct"],
        ["radiosxb", "checked", "checked"],
        ["destination", ["style", "display"], "none"],
        ["from_map", "placeholder", placeholderStrBx],
        ["to_map", "placeholder", placeholderStrBy],
        ["workday", "checked", "checked"]];
    foreach(eledata);
    var now_Expires = new Date();
    now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
    document.cookie = "createpintype=sxb;path=/;expires=" + now_Expires.toGMTString();
    LoadCityandLocation();
    document.getElementById("tocity").value = document.getElementById("fromcity").value;
    //window.sessionStorage.removeItem("createpintype");
    //window.sessionStorage.setItem("createpintype", "sxb");
    callback = funcSxb;
    tongcheng = [["workday", "none", "70%"],
        ["twoday", "none", "70%"],
        ["tempday", "", "35%"],
        ["eachday", "none", "70%"]];
    document.getElementById(typeselect).className = "";
    typeselect = "typeworkday";
    document.getElementById(typeselect).className = "selected";
});

function setAttr(el) {
    if (el[1] instanceof Array) {
        document.getElementById(el[0])[el[1][0]][el[1][1]] = el[2];
    } else {
        document.getElementById(el[0])[el[1]] = el[2];
    }
}

function foreach(eledata) {
    for (var i = 0; i < eledata.length; i++) {
        setAttr(eledata[i]);
    }
}

document.getElementById("ct").addEventListener("click", function () {
    this.className = "";
    var eledata = [["sxb", "className", "active"],
        //["ct_date", "className", "search"],
        ["sxb_date", "className", "search sxb"],
        ["radioct", "checked", "checked"],
        ["destination", ["style", "display"], "block"],
        ["from_map", "placeholder", placeholderStrX],
        ["to_map", "placeholder", placeholderStrX],
        ["workday", "checked", "checked"],
        ["startdate_date_temp", ["style", "display"], "none"],
        ["StartDate_Time", ["style", "width"], "70%"]];

    foreach(eledata);
    var now_Expires = new Date();
    now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
    document.cookie = "createpintype=ct;path=/;expires=" + now_Expires.toGMTString();
    LoadCityandLocation();
    //window.sessionStorage.removeItem("createpintype");
    //window.sessionStorage.setItem("createpintype", "ct");
    callback = funcCt;
    tongcheng = [["workday", "search ct", ""],
        ["twoday", "search ct", ""],
        ["tempday", "search", ""],
        ["eachday", "search ct", ""]];
    document.getElementById(typeselect).className = "";
    typeselect = "typeworkday";
    document.getElementById(typeselect).className = "selected";
});

/*
var arr = ["search", "create", "roboce", "searchLocation", "createLocation", "searchpintype", "createpintype", "cityhistory","create_null"];
for (var i = 0; i < arr.length; i++) {
    clearCookie(arr[i]);
}
*/
/*
var arr = ["create_null","create_ct","create_sxb","search_ct","search_sxb","createLocation","searchLocation"];

function clear(clearstr) {
    var cityhistory_expires = clearstr + "=;expires=Sat, 01-Jan-2000 00:00:00 GMT";
    document.cookie = cityhistory_expires + ";path=/;";
}
*/
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

setCreateType();

/*  */
//document.addEventListener("DOMContentLoaded", LoadCityandLocation);
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
    createpintype = returnCookie("createpintype"),
    whichtype = "city_" + createpintype;
    var temp = { fromcity: cityhistory === null ? "" : cityhistory, tocity: createpintype === null ? (cityhistory === null ? "" : cityhistory) : "" };
    //var temp = { fromcity: JSON.parse(window.localStorage.getItem("create")) === null ? null : JSON.parse(window.localStorage.getItem("create")).fromcity, tocity: null };
    var cookiecreate = returnCookie(whichtype);
    //if (!JSON.parse(window.sessionStorage.getItem("create"))) {
    if (!JSON.parse(cookiecreate)) {
        //var now_Expires = new Date();
        //now_Expires.setTime(now_Expires.getTime() + 24 * 60 * 60 * 1000);
        document.cookie = whichtype + "={\"fromcity\":\"" + temp.fromcity + "\",\"tocity\":\"" + temp.tocity + "\"};path=/;";
        //window.sessionStorage.setItem("create", JSON.stringify(temp));
    }
    //var sessionCity = JSON.parse(window.sessionStorage.getItem("create"));
    var sessionCity = JSON.parse(returnCookie(whichtype));
    if (sessionCity.fromcity === "") {
        loadLocation = true;
    }
    var eledata = [["fromcity", "value", unescape(sessionCity.fromcity)],
                    ["tocity", "value", unescape(sessionCity.tocity)]];
    foreach(eledata);
    //var sessionLocation = JSON.parse(window.sessionStorage.getItem("createLocation")) || { fromlocation: null, tolocation: null };
    var sessionLocation = (returnCookie("Location_" + createpintype) === null ? null : JSON.parse(returnCookie("Location_" + createpintype))) || { fromlocation: "", tolocation: "" };
    document.getElementById("from_map").value = unescape(sessionLocation.fromlocation);
    document.getElementById("to_map").value = unescape(sessionLocation.tolocation);

    //
    coordFrom = null;
    coordTo = null;
}

function setCreateType() {
    //var createpintype = window.sessionStorage.getItem("createpintype");
    var createpintype = returnCookie("createpintype");
    if (createpintype) {
        if (document.all) {
            document.getElementById(createpintype).click();
        } else {
            document.getElementById(createpintype).dispatchEvent(evt);
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
    //window.sessionStorage.removeItem("createLocation");
    var location = {
        fromlocation: escape(document.getElementById("from_map").value),
        tolocation: escape(document.getElementById("to_map").value)
    };
    if (arguments.length !== 0) {
        location[e] = "";
    }
    //location[e] = null;
    //window.sessionStorage.setItem("createLocation", JSON.stringify(location));
    //var whichtype = "createLocation_" + returnCookie("createpintype");
    var whichtype = "Location_" + returnCookie("createpintype");
    var now_Expires = new Date();
    now_Expires.setTime(now_Expires.getTime() + 30 * 24 * 60 * 60 * 1000);
    document.cookie = whichtype + "={\"fromlocation\":\"" + location.fromlocation + "\",\"tolocation\":\"" + location.tolocation + "\"};path=/;expires=" + now_Expires.toGMTString();
}

/* 始发地 城市 */
document.getElementById("fromcity_div").addEventListener("click", function () {
    showWait();
    saveLocation("fromlocation");
    window.location.href = window.location.protocol + "//" + window.location.host + "/route/city?citytype=fromcity&referrer=create";
});

/* 目的地 城市 */
document.getElementById("tocity_div").addEventListener("click", function () {
    showWait();
    saveLocation("tolocation");
    window.location.href = window.location.protocol + "//" + window.location.host + "/route/city?citytype=tocity&referrer=create";
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

for (var i = 0; i < tongcheng.length; i++) {
    (function (i) {
        document.getElementById(tongcheng[i][0]).addEventListener("click", function () { callback(tongcheng[i][1], tongcheng[i][2]); });
    })(i);
}

function funcSxb(style, width) {
    document.getElementById("StartDate_Time").style.width = width;
    document.getElementById("startdate_date_temp").style.display = style;
}

function funcCt(style, width) {
    document.getElementById("ct_date").className = style;
}