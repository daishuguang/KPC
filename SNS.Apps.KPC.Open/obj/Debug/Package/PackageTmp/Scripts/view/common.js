/*
	File: common.js
*/

/* Common Functions */
/*
	发送 ajax GET 请求
*/
function sendGetRequest(url, callback) {
	$.ajax({
		async: true,
		type: "GET",
		url: url,
		contentType: "application/json; charset=utf-8",
		data: "{}",
		dataType: "json",
		success: function (data, status) {
			if (status == "success") {
				if (typeof (callback) == "function") {
					callback(data);
				}
			}
		},
		error: function (response, status) {
			//alert("error: " + response.responseText);
		}
	});
}

/*
	发送 ajax POST 请求
*/
function sendPostRequest(url, data, callback) {
	$.ajax({
		async: true,
		type: "POST",
		url: url,
		contentType: "application/json; charset=utf-8",
		data: data,
		dataType: "json",
		processdata: true,
		success: function (data, status) {
			if (status == "success") {
				if (typeof (callback) == "function") {
					callback(data, status);
				}
			}
		},
		error: function (response, status) {
			//alert("error: " + response.responseText);
		}
	});
}

/*
	刷新当前页面
*/
function refreshPage() {
	window.location.reload(true);
}
/* End */