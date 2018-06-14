$(function () {

    $('#switch_qlogin').click(function () {
        $('#switch_login').removeClass("switch_btn_focus").addClass('switch_btn');
        $('#switch_qlogin').removeClass("switch_btn").addClass('switch_btn_focus');
        $('#switch_bottom').animate({ left: '0px', width: '70px' });
        $('#qlogin').css('display', 'none');
        $('#web_qr_login').css('display', 'block');

    });
    $('#switch_login').click(function () {

        $('#switch_login').removeClass("switch_btn").addClass('switch_btn_focus');
        $('#switch_qlogin').removeClass("switch_btn_focus").addClass('switch_btn');
        $('#switch_bottom').animate({ left: '154px', width: '70px' });

        $('#qlogin').css('display', 'block');
        $('#web_qr_login').css('display', 'none');
    });
    if (getParam("a") == '0') {
        $('#switch_login').trigger('click');
    }

});

function logintab() {
    scrollTo(0);
    $('#switch_qlogin').removeClass("switch_btn_focus").addClass('switch_btn');
    $('#switch_login').removeClass("switch_btn").addClass('switch_btn_focus');
    $('#switch_bottom').animate({ left: '154px', width: '96px' });
    $('#qlogin').css('display', 'none');
    $('#web_qr_login').css('display', 'block');

}


//根据参数名获得该参数 pname等于想要的参数名 
function getParam(pname) {
    var params = location.search.substr(1); // 获取参数 平且去掉？ 
    var ArrParam = params.split('&');
    if (ArrParam.length == 1) {
        //只有一个参数的情况 
        return params.split('=')[1];
    }
    else {
        //多个参数参数的情况 
        for (var i = 0; i < ArrParam.length; i++) {
            if (ArrParam[i].split('=')[0] == pname) {
                return ArrParam[i].split('=')[1];
            }
        }
    }
}


var reMethod = "GET",
	pwdmin = 6;
var publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDVt2HeAt5gvPIBDSviQh7tjKoNfnYNBTwMRYIk43JGPCbQeNf/pA0gbBMyDXCihcS2xmZd7ag9lOaXkR79k/WSMa7pCuHaDenWGCwDDqB6F4/iTWRoKA1g0MME0i1+uXnA+RV3UrZ0Jm7yY4d/GtkWOh92alzPG/kViObhKtV4PQIDAQAB";
$(document).ready(function () {
    function loginsystem() {
        var email = $('#email').val();
        var password = $('#password').val();
        var encrypt = new JSEncrypt();
        encrypt.setPublicKey(publickey);
        password = encrypt.encrypt(password);
        $.ajax({
            url: "../ProxyHandler/LoginHandler.ashx",
            type: "POST",
            data: {
                type: "login",
                userid: email,
                password: password
            },
            dataType: "json",
            success: function (result) {
                var data = result.data;
                if (data.length) {
                    $.cookie('gisuserid', email, { expires: 7 });
                    $.cookie('gisusername', data[0].UserName, { expires: 7 });
                    window.location = 'index.html';
                } else {
                    layer.msg('登录失败', { icon: 2 });
                }
            },
            error: function (e) {
                layer.msg('网络错误', { icon: 2 });
            }
        });
    }

    function registersystem() {
        var email = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+/;
        if (!email.test($('#userid').val()) || $('#userid').val().length < 5 || $('#userid').val().length > 24) {
            $('#userid').focus().css({
                border: "1px solid red",
                boxShadow: "0 0 2px red"
            });
            $('#userCue').html("<font color='red'><b>×邮箱格式不正确</b></font>");
            return false;
        } else {
            $('#userid').css({
                border: "1px solid #D7D7D7",
                boxShadow: "none"
            });

        }
        if ($('#username').val() == "") {
            $('#username').focus().css({
                border: "1px solid red",
                boxShadow: "0 0 2px red"
            });
            $('#userCue').html("<font color='red'><b>×用户名不能为空</b></font>");
            return false;
        }
        if ($('#username').val().length < 4 || $('#username').val().length > 16) {

            $('#username').focus().css({
                border: "1px solid red",
                boxShadow: "0 0 2px red"
            });
            $('#userCue').html("<font color='red'><b>×用户名位4-16字符</b></font>");
            return false;
        }

        if ($('#passwd').val().length < pwdmin) {
            $('#passwd').focus();
            $('#userCue').html("<font color='red'><b>×密码不能小于" + pwdmin + "位</b></font>");
            return false;
        }
        if ($('#repassword').val() != $('#passwd').val()) {
            $('#repassword').focus();
            $('#userCue').html("<font color='red'><b>×两次密码不一致！</b></font>");
            return false;
        }
        var userid = $('#userid').val();
        var username = $('#username').val();
        var passwd = $('#passwd').val();
        var encrypt = new JSEncrypt();
        encrypt.setPublicKey(publickey);
        passwd = encrypt.encrypt(passwd);
        $.ajax({
            url: "../ProxyHandler/LoginHandler.ashx",
            type: "POST",
            data: {
                type: "register",
                userid: userid,
                username: username,
                password: passwd
            },
            dataType: "json",
            success: function (result) {
                var data = result.data;
                if (data) {
                    $('#switch_login').removeClass("switch_btn_focus").addClass('switch_btn');
                    $('#switch_qlogin').removeClass("switch_btn").addClass('switch_btn_focus');
                    $('#switch_bottom').animate({ left: '0px', width: '70px' });
                    $('#qlogin').css('display', 'none');
                    $('#web_qr_login').css('display', 'block');
                    $('#email').val(userid);
                    layer.msg('注册成功', { icon: 1 });
                } else {
                    layer.msg('注册失败', { icon: 2 });
                }
            },
            error: function (e) {
                layer.msg('网络错误', { icon: 2 });
            }
        });
    }
    $('#password').on('keydown', function (e) {
        var evt = document.all ? window.event : e;
        if (evt.keyCode == 13) {
            loginsystem();
        }
    });
    $('#repassword').on('keydown', function (e) {
        var evt = document.all ? window.event : e;
        if (evt.keyCode == 13) {
            registersystem();
        }
    });
    $('#login').click(function () {
        loginsystem();
    });
    $('#register').click(function () {
        registersystem();
    });
});