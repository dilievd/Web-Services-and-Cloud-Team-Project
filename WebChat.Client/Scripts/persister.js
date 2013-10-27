/// <reference path="../jquery-2.0.2.js" />
/// <reference path="http-requester.js" />
/// <reference path="Class.js" />
/// <reference path="http://crypto-js.googlecode.com/svn/tags/3.1.2/build/rollups/sha1.js" />

var persister = (function () {
    var authCode = localStorage.getItem('authCode');
    var mainUrl = '';

    var mainPersister = Class.create({
        init: function (url) {
            this.url = url;
            mainUrl = url;
            this.user = new user(this.url);
            this.chat = new chat();
            this.messages = new messages();
        },
        isUserLoggedIn: function () {
            isLoggedIn = (localStorage.getItem('authCode') != '' && localStorage.getItem('authCode') != undefined);
            return isLoggedIn;
        }
    });

    var user = Class.create({
        init: function (url) {
            this.url = url + '/users'
        },
        register: function (username, nickname, password, error) {
            var userData = {
                "username": username,
                "nickname": nickname,
                "authCode": CryptoJS.SHA1(password).toString()
            };

            var url = this.url + '/register/' + username + '/' + userData.authCode;

            httpRequester.getJson(url,
                function (data) {
                    localStorage.setItem('authCode', data.SessionKey);
                    localStorage.setItem('userNickname', data.Name);
                    localStorage.setItem('userId', data.Id);
                },
                error);
        },

        login: function (username, password, error) {
            var userData = {
                "username": username,
                "authCode": CryptoJS.SHA1(password).toString(),
            };

            var url = this.url + '/login/' + username + '/' + userData.authCode;

            httpRequester.getJson(url,
                function (data) {
                    localStorage.setItem('authCode', data.SessionKey);
                    localStorage.setItem('userNickname', data.Name);
                    localStorage.setItem('userId', data.Id);
                    localStorage.setItem('avatarURL', data.AvatarURL);
                    console.log(data);
                },
                error);
        },

        logout: function () {
            var url = this.url + '/logout/';

            if (true) {
                url = url + localStorage.getItem('authCode');
            }

            httpRequester.getJson(url,
                function () {
                    //localStorage.setItem('authCode', '');
                    //localStorage.setItem('userNickname', '');
                    //localStorage.setItem('userId', '');
                    //localStorage.setItem('avatarURL', '');
                    localStorage.clear();
                },
                function () { console.log('Try again') });
        },
        all: function (success) {
            var url = this.url;

            httpRequester.getJson(url,
                success,
                function () { console.log('Try again') });
        }
    });

    var chat = Class.create({
        init: function () {
            this.url = mainUrl + '/chats'
        },
        create: function (id, success, error) {
            var url = this.url + '/new/' + id + '/' + localStorage.getItem('authCode');

            httpRequester.getJson(url, success, error);
        },
        sendMessage: function (id, content, success, error) {
            var userModel = { Id: id };
            var message = { Owner: userModel, Content: content };

            var url = this.url + '/senMessage/' + id + '/' + localStorage.getItem('authCode');

            httpRequester.postJson(url, null, success, error);
        },
        all: function (success, error) {
            var url = this.url + '/' + localStorage.getItem('authCode');

            httpRequester.getJson(url, success, error);
        }
    });

    var messages = Class.create({
        init: function () {
            this.url = mainUrl + '/messages';
        },
        all: function (chatId, success, error) {
            var url = this.url + '/' + chatId + '/' + localStorage.getItem('authCode');

            httpRequester.getJson(url, success, function (err) { console.log(err); });
        }
    });

    return {
        mainPersister: function (url) { return new mainPersister(url) },
        nickname:  localStorage.getItem('userNickname'),
        authCode: localStorage.getItem('authCode')
    }
}());