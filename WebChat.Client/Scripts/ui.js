/// <reference path="../jquery-2.0.2.js" />
/// <reference path="persister.js" />

var ui = (function () {
    function drawLogInForm() {
        return '<fieldset id="login-user-container">' +
                    '<legend>Log In</legend>' +
                    '<label for="login-user-nickname">Nickname</label>' +
                    '<input id="login-user-nickname" type="text" name="name" value="" placeholder="Enter your nickname" autofocus="true" />' +
                    '<label for="login-user-password">Password</label>' +
                    '<input id="login-user-password" type="password" name="name" value="" placeholder="Enter your password" />' +
                    '<button id="button-log-in">Log In</button>' +
                    '<a id="register-now" href="#">Register now</a>' +
                '</fieldset>';
    }

    function drawRegisterForm() {
        return '<fieldset id="register-user-container">' +
                    '<legend>Register</legend>' +
                    '<label for="register-user-name">Name</label>' +
                    '<input id="register-user-name" type="text" name="name" value="" placeholder="Enter your first and last name" autofocus="true" />' +
                    '<label for="register-user-nickname">Nickname</label>' +
                    '<input id="register-user-nickname" type="text" name="name" value="" placeholder="Enter your nickname" />' +
                    '<label for="register-user-password">Password</label>' +
                    '<input id="register-user-password" type="password" name="name" value="" placeholder="Enter your password" />' +
                    '<label for="register-user-password-re">Password</label>' +
                    '<input id="register-user-password-re" type="password" name="name" value="" placeholder="Confirm your password" />' +
                    '<a id="back-to-homepage" href="#" >Back</a>' +
                    '<button id="button-register">Register</button>' +
                '</fieldset>';
    }

    function drawLoggedInForm() {
        return '<div id="user-loged-in">' +
                    '<p id="greetings">Hello, ' + localStorage.getItem('userNickname') + '</p>' +
                    drawImageOrAskForUpload() +
                    '<p>Let\'s chat ...</p>' +
                    '<button id="button-logout">Log out</button>' +
                '</div>';
    }

    function drawImageOrAskForUpload() {
        var avatarUrl = localStorage.getItem('avatarURL');
        
        if (avatarUrl && avatarUrl != "null" && avatarUrl !="" && avatarUrl != null) {
            return '<img id="avatar" width="80" height="80" src="' + avatarUrl + '" alt="avatar"/>';
        }
        else {
            return drawUploadAvatarForm();
        }
    }

    function drawUserInteraction() {
        return '<div id="error-messages"></div>' +
               '<div id="messages"></div>' +
               '<div id="current-chat-container"></div>';
    }

    function drawUploadAvatarForm() {
        return '<span class="btn btn-success fileinput-button">' +
            '<i class="icon-plus icon-white"></i>' +
            '<span>Upload image...</span>' +
            '<input id="fileupload" type="file" accept="image/*">' +
        '</span>' +
        '<br>' +
        '<br>' +
        '<div style="display:none" id="progress" class="progress progress-success progress-striped">' +
            '<div class="bar"></div>' +
        '</div>';
    }

    function showAppErrorMessage(message) {
        $('#error-messages').text(message);

        setTimeout(function () {
            $('#error-messages').text('');
        }, 15000);
    }

    function showErrorMessage(err) {
        $('#error-messages').text(err);


        setTimeout(function () {
            $('#error-messages').text('');
        }, 15000);
    }

    function showMessage(message) {

        $('#messages').text(message);

        setTimeout(function () {
            $('#messages').text('');
        }, 15000);
    }

    function clearErrorMessage() {
        $('#error-messages').text('');
    }

    function showOrHideElements(elementID) {
        if ($(elementID + '-list').hasClass('show')) {
            $(elementID + '-list').hide(1500);
            $(elementID + '-list').removeClass('show');
        }
        else {
            $(elementID + '-list').show(1500);
            $(elementID + '-list').addClass('show');
        }

        return false;
    }

    function drawListOfChats(myActiveChats, container, conatinerTitle, type) {
        $(container)
            .append($('<h2 />').attr('id', 'chats-' + type).text(conatinerTitle))
            .append($('<ul />').attr({ 'id': 'chats-' + type + '-list', 'class': 'show' }));

        var elementsContainer = $(container + ' #' + 'chats-' + type + '-list');

        var currentUser = localStorage.getItem('userId');

        for (var i = 0; i < myActiveChats.length; i++) {
            var otherUser;
            var otherUserId;

            if (myActiveChats[i].User1.Id != currentUser) {
                otherUser = myActiveChats[i].User1.Name;
                otherUserId = myActiveChats[i].User1.Id;
            }
            else {
                otherUser = myActiveChats[i].User2.Name;
                otherUserId = myActiveChats[i].User2.Id;
            }

            var currentLi = $('<li />').attr({
                'data-chat-id': myActiveChats[i].Id,
                'data-chat-channel': myActiveChats[i].Channel,
                'data-user-id': otherUserId
            }).append($('<a />').attr('href', '#').text(otherUser));

            elementsContainer.append(currentLi);
        }
    }

    function drawListOfUsers(users, container, conatinerTitle, type) {
        $(container)
            .append($('<h2 />').attr('id', 'users-' + type).text(conatinerTitle))
            .append($('<ul />').attr({ 'id': 'users-' + type + '-list', 'class': 'show' }));

        var elementsContainer = $(container + ' #' + 'users-' + type + '-list');

        for (var i = 0; i < users.length; i++) {
            elementsContainer
                .append($('<li />').attr('data-user-id', users[i].Id).append($('<a />').attr('href', '#').text(users[i].Name)));
        }
    }

    function drawSendMessageMenu(otherUserID, chatId, channelID) {
        return '<div id="chat-user">' +
                    '<input id="message-text" type="text" name="name" value="" placeholder="Message" />' +
                    '<button id="confirm-send" data-user-id="' + otherUserID + '" data-chat-id="' + chatId + '" data-channel-id="' + channelID + '">Send</button>' +
                '</div>';
    }

    function drawMessages(data) {
        $('#current-chat-state').empty();
        var elementsContainer = $('#current-chat-state').append($('<ul />'));
        elementsContainer = $('#current-chat-state ul')

        for (var i = 0; i < data.length; i++) {
            elementsContainer
                .append($('<li />').attr('data-owner-id', data[i].OwnerId).text(data[i].OwnerName + ': ' + data[i].Content));
        }
    }

    function drawSidebars(persister) {
        $('#left-side-bar').empty();
        $('#right-side-bar').empty();
        persister.chat.all(function (data) {
            drawListOfChats(data, '#left-side-bar', 'My active chats', 'active');
        });

        persister.user.all(function (data) {
            drawListOfUsers(data, '#right-side-bar', 'Users', 'user');
        });
    }

    return {
        drawLogIn: drawLogInForm,
        drawRegister: drawRegisterForm,
        drawLoggedIn: drawLoggedInForm,
        drawUserInteraction: drawUserInteraction,
        showAppErrorMessage: showAppErrorMessage,
        showErrorMessage: showErrorMessage,
        showMessage: showMessage,
        clearErrorMessage: clearErrorMessage,
        drawListOfChats: drawListOfChats,
        drawListOfUsers: drawListOfUsers,
        drawSendMessageMenu: drawSendMessageMenu,
        drawMessages: drawMessages,
        drawSidebars: drawSidebars,
        showOrHideElements: showOrHideElements
    };
}());