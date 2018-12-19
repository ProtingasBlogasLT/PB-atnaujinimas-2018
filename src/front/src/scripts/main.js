var apiHost = "127.0.0.1";
var apiPort = "5000";
var apiUrl = "http://" + apiHost + ":" + apiPort;

function setToken(token)
{
    window.localStorage.setItem("token", token);
}

function getToken()
{
    return window.localStorage.getItem("token");
}

function loginSuccess(data)
{
    setToken(data.token);
    window.location.href = "index.html";
}

function loginError(data)
{
    console.log("login error");
}

function login()
{
    var username = $("#username").val();
    var password = $("#password").val();

    var user = {
        username: username,
        password: password
    };
    $.ajax({
        contentType: "application/json",
        dataType: "json",
        url: apiUrl + "/users/authenticate",
        method: "POST",
        crossDomain: true,
        data: JSON.stringify(user),
        success:loginSuccess,
        error:loginError
    });
}

function registerSuccess(data)
{
    window.location.href = "login.html";
}

function registerError(data)
{
    console.log("register error");
}

function register()
{
    var username = $("#username").val();
    var password = $("#password").val();
    var password2 = $("#password").val();
    if(password !== password2)
    {
        return;
    }

    var user = {
        username: username,
        password: password
    };
    $.ajax({
        contentType: "application/json",
        dataType: "json",
        url: apiUrl + "/users/register",
        method: "POST",
        crossDomain: true,
        data: JSON.stringify(user),
        success:registerSuccess,
        error:registerError
    });
}

function articlesSuccess(data)
{
    var template = $("#article-template");
    var lastArticle = template;
    console.log(data);
    for(var i = 0; i < data.length; ++i)
    {
        var articleData = data[i];
        var article = template.clone(true);
        article.removeAttr("id");
        article.removeClass("hide");
        article.find(".panel-title").text(articleData.name);
        article.find(".panel-body").text(articleData.text);
        lastArticle.after(article);
        lastArticle = article;
        console.log(article);
    }
}

function articlesError(data)
{
    window.location.href = "login.html";
}

$(document ).ready(function() {
    var articles = $("#article-template");
    if(articles.length == 0)
    {
        return;
    }
    $.ajax({
        dataType: "json",
        url: apiUrl + "/api/articles",
        method: "GET",
        crossDomain: true,
        headers:{
            "Authorization": "Bearer " + getToken()
        },
        success:articlesSuccess,
        error:articlesError
    });
});