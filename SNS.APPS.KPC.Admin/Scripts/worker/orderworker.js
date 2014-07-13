var i = 0;
var intervalTime;

self.addEventListener('message', function (e) {
    var data = e.data;
    switch (data.cmd) {
        case 'setIntervalTime':
            intervalTime = data.msg;
            break;
    };
}, false);
setTimeout("checkOrders()", intervalTime);
function checkOrders() {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "/Order/CheckOrderByDatetime");
    xhr.onload = function () {
        if (xhr.readyState == 4) {
            postMessage(xhr.responseText);
        }
    };
    xhr.send();
    setTimeout("checkOrders()", intervalTime); 
}
//checkOrders();

