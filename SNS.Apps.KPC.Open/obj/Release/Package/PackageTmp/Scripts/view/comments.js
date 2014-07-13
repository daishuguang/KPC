/*
Script File: comment.js
*/

/*
	
*/
function submitComments(owner, userID, routeID, scoreByUserID, scoreByRouteID, scoreRate) {
	$(owner).parent().children("a.ui-btn").addClass("ui-disabled");

	var comments = $.trim($(owner).parents("li").find("textarea").val());
	var data = '{ "userID": "' + userID + '", "routeID": "' + routeID + '", "scoreByUserID": "' + scoreByUserID + '", "scoreByRouteID": "' + scoreByRouteID + '", "scoreRate": "' + scoreRate + '", "comments": "' + escape(comments) + '", "signcode": "' + _signCode + '" }';

	sendPostRequest(
		_serviceEntry + "/SubmitComments",
		data,
		function () {
			$(owner).parents("li").find("textarea").textinput("disable");
			$(owner).parent().remove();
		}
	);
}


/*
	
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