var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, imageUrl, leftOrRight) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = message

    if (leftOrRight == "left") {
        var divMessageItem = document.createElement("div");
        divMessageItem.classList.add("message_item");

        var divMessageItemFriendText = document.createElement("div");
        divMessageItemFriendText.classList.add("message_item_friend_text");

        var divMessageItemImgContainer = document.createElement("div");
        divMessageItemImgContainer.classList.add("message_item_img_container");
        var img = document.createElement("img");
        img.src = imageUrl; // Thay đổi đường dẫn ảnh tùy theo imageUrl
        img.alt = "user_image";
        divMessageItemImgContainer.appendChild(img);

        var pTextLeft = document.createElement("p");
        pTextLeft.classList.add("text_left");
        pTextLeft.textContent = encodedMsg;

        // Ghép các phần tử vào nhau
        divMessageItemFriendText.appendChild(divMessageItemImgContainer);
        divMessageItemFriendText.appendChild(pTextLeft);
        divMessageItem.appendChild(divMessageItemFriendText);

        // Thêm div vào danh sách tin nhắn
        document.getElementById("messages_display_container").appendChild(divMessageItem);
        // Tạo các phần tử HTML
    } else {
        var divMessageItem = document.createElement("div");
        divMessageItem.classList.add("message_item");

        var divMessageItemFriendText = document.createElement("div");
        divMessageItemFriendText.classList.add("message_item_user_text");

        var divMessageItemImgContainer = document.createElement("div");
        divMessageItemImgContainer.classList.add("message_item_img_container");
        var img = document.createElement("img");
        img.src = imageUrl; // Thay đổi đường dẫn ảnh tùy theo imageUrl
        img.alt = "user_image";
        divMessageItemImgContainer.appendChild(img);

        var pTextLeft = document.createElement("p");
        pTextLeft.classList.add("text_right");
        pTextLeft.textContent = encodedMsg;

        // Ghép các phần tử vào nhau
        divMessageItemFriendText.appendChild(divMessageItemImgContainer);
        divMessageItemFriendText.appendChild(pTextLeft);
        divMessageItem.appendChild(divMessageItemFriendText);

        // Thêm div vào danh sách tin nhắn
        document.getElementById("messages_display_container").appendChild(divMessageItem);
    }
});
function handleSendMessage(event) {
    if (event.keyCode == 13) {
        if (!event.shiftKey) {
            var user = document.getElementById("userInput").value;
            var receiverConnectionId = document.getElementById("receiverId").value;
            var message = document.getElementById("messageInput").value;
            connection.invoke("SendToUser", user, receiverConnectionId, message).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        }
    }
}
//connection.on("ReceiveMessage", function (user, message, imageUrl) {
//    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
//    var encodedMsg = user + " says " + msg;
//    var li = document.createElement("li");
//    li.textContent = encodedMsg;
//    document.getElementById("messagesList").appendChild(li);
//});

connection.start().then(function () {
    console.log("Connected");
    connection.invoke("GetConnectionId").then(function (id) {
        //document.getElementById("connectionId").innerText = id;
    });
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

if (document.getElementById("sendButton")) {
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
}

document.getElementById("sendToUser").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var receiverConnectionId = document.getElementById("receiverId").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendToUser", user, receiverConnectionId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});