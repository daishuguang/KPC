var KPC = {};
KPC.EditProfile = {};
KPC.EditProfile.r = new RegExp(/^[1][358][0-9]{9}$/);

$(document).ready(function () {
    var phoneRegExp = new RegExp(/^[1][358][0-9]{9}$/), count = 30, clickCount = 0, flag = 1;
    $("#sub").on("click", function () {
        var flag = true;
        //var userrole = $("input:radio:checked").val();
        $("input:text").each(function () {
            if ($(this).attr("required") !== undefined) {
                if ($.trim($(this).val()) === "") {
                    $(this).css("border", "1px solid red");
                    flag = false;
                }
            }

            if (this.id === "mobilenum") {
                flag = KPC.EditProfile.r.test($(this).val());
                if (!flag) {
                    $(this).css("border", "1px solid red");
                    $("#phone_error").css("display", "block");
                }
            }
            /*
            if (userrole === "Driver") {
                var $el = $("#txt_licenceplatnumber");
                flag = $.trim($el.val()) === "" ? false : flag;
                if (!flag) {
                    $el.css("border", "1px solid red");
                }
            }
			*/
        });
        if (document.getElementById("txt_refcode") !== null) {
            if (document.getElementById("txt_refcode").value !== "" && document.getElementById("txt_refcode").value.length !== 6) {
                alert("亲，推荐码是6位");
                return;
            }
        }
        if (flag) {
            if (document.getElementById("mobilenum").value !== document.getElementById("telorigin").value) {
                $("#input_ismodified").val("true");
            }
            showWait();
            $("form").submit();
            $(this).prop("disabled", true);
        }
    });

    $("#mobilenum").on("change", function () {
        if ($.trim($(this).val()) !== "") {
            $(this).css("border", "");
        }
        /* validate mobilenum */
        var flag = true;
        if (this.id === "mobilenum") {
            flag = KPC.EditProfile.r.test($(this).val());
            if (!flag) {
                $(this).css("border", "1px solid red");
                $("#phone_error").css("display", "block");
            } else {
                $("#phone_error").css("display", "none");
            }
        }
    });

    $("input:radio").on("change", function () {
        var $li_licencePlateNumber = $("input:radio:checked").attr("id");
        switch ($li_licencePlateNumber) {
            case "radiodriver":
                $("#lblpassenger").removeClass("yeschecked").addClass("notchecked");
                $("#lbldriver").removeClass("notchecked").addClass("yeschecked");
                $("#li_licencePlateNumber").css("display", "block");
                break;
            case "radiopassenger":
                $("#lblpassenger").removeClass("notchecked").addClass("yeschecked");
                $("#lbldriver").removeClass("yeschecked").addClass("notchecked");
                $("#li_licencePlateNumber").css("display", "none");
                break;
            default:
                break;
        }
        $("#txt_licenceplatnumber").css("border", "");
    });

    /*
    * 修改手机号 
    */
    $("#spanmodify").on("click", function () {
        var $datamodify = $(this).attr("data-modify");
        switch ($datamodify) {
            case "notmodify":
                $(this).text("放弃修改");
                $(this).attr("data-modify", "modify");
                $("#mobilenum").removeAttr("readonly").focus().removeClass("tel");
                $("#tr_phone").css("display", "");
                break;
            case "modify":
                $(this).text("点击修改");
                $(this).attr("data-modify", "notmodify");
                $("#mobilenum").val($("#telorigin").val()).attr("readonly", "readonly").trigger("change").addClass("tel");
                $("#tr_phone").css("display", "none");
                break;
            default:
                break;
        }
    });

    /*
    * 获取验证码
    */
    $("#btnCode").on("click", setCode);

    /*
    * el: btnCode
    * event: click
    * func: set code
    */
    function setCode(e) {
        var phone = $.trim($("#mobilenum").val());
        if (!phoneRegExp.test(phone)) {
            alert("亲，您的手机号填错了吧");
            return;
        }
        if (phone === $("#telorigin").val()) {
            alert("请输入新的手机号");
            return;
        }
        if (clickCount >= 10) {
            alert("请回复微信，联系我们的客服专员");
            return;
        } else {
            clickCount++;
        }
        count = 30;
        $(e.currentTarget).off("click");
        getNumber();

        $.ajax({
            url: "/api/verifyphonenumber",
            type: "POST",
            data: JSON.stringify({ phonenum: phone, channel: flag }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (data, textStatus, jqXHR) {
            if (!data.flag) {
                clearTimeout(seed);
                $("#btnCode").text("重新获取");
                document.getElementById("btnCode").className = "verify_able";
                $("#btnCode").on("click", setCode);
            }
        }).fail(function (e) {
            clearTimeout(seed);
            $("#btnCode").text("重新获取");
            document.getElementById("btnCode").className = "verify_able";
            $("#btnCode").on("click", setCode);
        });
        flag = (flag === 0 ? 1 : 0);
    }

    /*
    * 
    * func: 设置90秒后重新发送
    */
    function getNumber() {
        count--;
        $("#btnCode").text(count + "秒后可重发");
        document.getElementById("btnCode").className = "verify_disable";
        if (count > 0) {
            seed = setTimeout(getNumber, 1000);
        } else {
            $("#btnCode").text("重新获取");
            document.getElementById("btnCode").className = "verify_able";
            $("#btnCode").on("click", setCode);
        }
    }

    $("input:radio").trigger("change");

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
});