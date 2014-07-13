//var msgIntervalTime = 60000;
var snsworker; //web worker
var msgIntervalTime
var initWorkerIntervalTime = 10000;
if (localStorage.SNS_order_interval) {
    msgIntervalTime = localStorage.SNS_order_interval
}
else {
    msgIntervalTime = 60000;
}

function setOrderIntervalTime(value) {
    msgIntervalTime = value;
    localStorage.SNS_order_interval = value;
}

function getOrderIntervalTime() {
    return msgIntervalTime;
}

function showAlertMsg(title, msg,type)
{
    $.messager.alert(title, msg, type);
}

function showProcess()
{
    $.messager.progress()
}

function closeProcess() {
    $.messager.progress('close');
}


function slideShow(title, msg, timeout) {
    $.messager.show({
        title: title,
        msg: msg,
        timeout: timeout,
        showType: 'slide'
    });
}


function ConvertDateTimeInGrid(selector) {
    $(selector).each(function (index, e) {
        if (index == 0 || $(this).text() == "")
            return;
        var datetime = ConvertJSONDateToJSDateObject($(this).text());
        $(this).text(getDateTime(datetime));
    });
}

//json datetime conversion
function ConvertJSONDateToJSDateObject(jsondate) {
    var date = new Date(parseInt(jsondate.replace("/Date(", "").replace(")/", ""), 10));
    return date;
}


function getDate(date) {
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    return year + "-" + month + "-" + day;
}

function getDateTime(date) {
    if (date == "" || date == undefined)
        return;
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hh = date.getHours();
    var mm = date.getMinutes();
    var ss = date.getSeconds();
    return year + "-" + month + "-" + day + " " + hh + ":" + mm + ":" + ss;
}