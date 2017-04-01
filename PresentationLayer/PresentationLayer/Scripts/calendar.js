//reservedTimeslots color pallette
var colorPallette = [
    ['#16a085', '#1abc9c'],
    ['#27ae60', '#2ecc71'],
    ['#2980b9', '#3498db'],
    ['#8e44ad', '#9b59b6'],
    ['#2c3e50', '#34495e'],
    ['#f39c12', '#f1c40f'],
    ['#d35400', '#e67e22'],
    ['#c0392b', '#e74c3c'],
    ['#7f8c8d', '#95a5a6']
];


$(window).scroll(function () {
    $('.room').css('left', 0 - $(this).scrollLeft());
});
$(".reservation-popup-test").draggable();
//header calendar 
var date = new Date();

var months = ["JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER"];
var days = ["SUNDAY","MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY"];

$(".next").click(function () {
    var day = parseInt($(".upper-header li .date .day").html());
    date.setDate(day + 1);
    $(".upper-header li .date .day").html(date.getDate());
    $(".upper-header li .date .month").html(months[date.getMonth()]);
    $(".upper-header li .dayOfTheWeek").html(days[date.getDay()]);
    setCalendarDate();
    $(".getNext").click();
});

$(".prev").click(function () {
    var day = parseInt($(".upper-header li .date .day").html());
    date.setDate(day - 1);
    $(".upper-header li .date .day").html(date.getDate());
    $(".upper-header li .date .month").html(months[date.getMonth()]);
    $(".upper-header li .dayOfTheWeek").html(days[date.getDay()]);
    setCalendarDate();
    $(".getNext").click();
    
});

$(".numOfComputers").click(function () {
    $("input[name='day']").attr("value", numOfComputers);
});

$(".numOfProjectors").click(function () {
    $("input[name='day']").attr("value", numOfComputers);
});

$(".numOfMarkers").click(function () {
    $("input[name='day']").attr("value", numOfComputers);
});
function setCalendarDate() {
    $("input[name='day']").attr("value", (date.getDate()));
    $("input[name='month']").attr("value", (date.getMonth() + 1));
    $("input[name='year']").attr("value", (date.getFullYear()));
    
}



function rendercalendar() {
}

//Function is run when any of the timeslot li is clicked
function timeslotClicked(event) {
    if (!$(".custom-navbar-right .icon").hasClass("dropdownLogout"))
    {
        notLoggedIn();

    } 
    else {

    
    var seconfuncCalled = false;
    var thisElement = event;
    var room = $(event).data("room");
    $("input[name='room']").attr("value", room);
    setCalendarDate();
    var timeslot = $(event).data("timeslot");
    var firstAndLastTimeslot = [timeslot, timeslot];
    $("#firstTimeslot").html(firstAndLastTimeslot[0]);
    $("#lastTimeslot").html(firstAndLastTimeslot[0] + 1);
    funcCalled = true;
    $(event).addClass("active");
    $(".reservation-popup-test h1").html("Select another timeslot");
    $(".reservation-popup-test").toggle(0);
    $(".reservation-popup-test").css('opacity', '0');
    $(".reservation-popup-test").position({
        my: "left top",
        at: "right+7 top+-7",
        of: thisElement,
    });
    $(".reservation-popup-test").toggle(0);
    $(".reservation-popup-test").css('opacity', '1');
    $(".reservation-popup-test").toggle(300);
    $(".timeslots li ul li").off("click.firstFunction");
    $(".timeslots li ul li").on("click.secondFunction", function () {

        var timeslot2 = $(this).data("timeslot");
/*
        if (seconfuncCalled == false) {
            firstAndLastTimeslot[1] = firstAndLastTimeslot[0];
            seconfuncCalled = true;
        }
*/
        if ($(this).attr('data-room') == room) {
            //if timeslot selected at the begining or after the range
            if (firstAndLastTimeslot[1] <= timeslot2 ) {
                if (timeslot2 - firstAndLastTimeslot[0] > 3) {
                    firstAndLastTimeslot[1] = firstAndLastTimeslot[0] + 3;
                }
                else {
                    firstAndLastTimeslot[1] = timeslot2;
                }
                for (var i = firstAndLastTimeslot[0] + 1; i <= firstAndLastTimeslot[1]; i++) {
                    //adds more timeslots to the already active tismeslots
                    if ($("li[data-timeslot='" + i + "']li[data-room='" + room + "']").hasClass("active"));
                    else {
                        //toggles active the desired timesots begining
                        $("li[data-timeslot='" + i + "']li[data-room='" + room + "']").toggleClass("active");
                    }
                }
               
            }
            else {
                //if timeslot is selected before the range
                if (firstAndLastTimeslot[0] > timeslot2) {

                    if (firstAndLastTimeslot[1] - timeslot2 <= 3) {
                        firstAndLastTimeslot[0] = timeslot2;
                    }
                    else {
                        firstAndLastTimeslot[0] = firstAndLastTimeslot[1] - 3;
                    }
                    for (var i = firstAndLastTimeslot[0]; i <= firstAndLastTimeslot[1]; i++) {
                        //adds more timeslots to the already active tismeslots
                        if ($("li[data-timeslot='" + i + "']li[data-room='" + room + "']").hasClass("active")) { }
                        else {
                            //toggles active the desired timesots
                            $("li[data-timeslot='" + i + "']li[data-room='" + room + "']").toggleClass("active");
                        }
                    }
                }
                    //if timeslot selected is between the range that was already selected
                else {
                    for (var i = parseInt(timeslot2) + 1; i <= parseInt(firstAndLastTimeslot[1]) ; i++) {
                        $("li[data-timeslot='" + i + "']li[data-room='" + room + "']").toggleClass("active");
                    }
                    firstAndLastTimeslot[1] = timeslot2;
                }

            }

            //firstAndLastTimeslot[1] = $(this).data("timeslot");
            $("#firstTimeslot").html(firstAndLastTimeslot[0]);
            $("input[name='firstTimeslot']").attr("value", firstAndLastTimeslot[0]);

            $("#lastTimeslot").html(firstAndLastTimeslot[1] + 1);
            $("input[name='lastTimeslot']").attr("value", firstAndLastTimeslot[1]);


        }
    });
    
    $("#firstTimeslot").html(firstAndLastTimeslot[0]);
    $("input[name='firstTimeslot']").attr("value", firstAndLastTimeslot[0]);

    $("#lastTimeslot").html(firstAndLastTimeslot[1] + 1);
    $("input[name='lastTimeslot']").attr("value", firstAndLastTimeslot[1]);

    }
}

$(".timeslots li ul li").on("click.firstFunction", function () {
    timeslotClicked(this);
});

//Function is run when cancel button is clicked
$(".reservation-popup-test .header span").click(function () {
    $(".reservation-popup-test").toggle(250);
    $(".timeslots li ul li").off("click.secondFunction");
    $(".timeslots li ul li").on("click.firstFunction", function () {
        timeslotClicked(this);
    });
    $(".timeslots .active").toggleClass("active");

});
var serverSession = $.connection.calendarHub;
//Jquery to update the timeslots
serverSession.client.updateCalendar = updateCalendar;


function updateCalendar(reservationList) {
    remakeCalendar();
    for (j = 0; j < reservationList.length; j++) {
        var color = colorPallette[reservationList[j].userId];
        for (var i = reservationList[j].initialTimeslot; i <= reservationList[j].finalTimeslot; i++) {
            $("li[data-timeslot='" + i + "']li[data-room='" + reservationList[j].roomId + "']").addClass("reserved");
            $("li[data-timeslot='" + i + "']li[data-room='" + reservationList[j].roomId + "']").html("");
            $("li[data-timeslot='" + i + "']li[data-room='" + reservationList[j].roomId + "']").css('background-color', color[1]);
        }
        //First timeslot classtoggle=reservedHeader
        //Add first Name
        $("li[data-timeslot='" + (reservationList[j].initialTimeslot) + "']li[data-room='" + reservationList[j].roomId + "']").addClass("reserved-header").html('<div class="reserved-left">'+reservationList[j].userName+'</div>');
        $("li[data-timeslot='" + (reservationList[j].initialTimeslot) + "']li[data-room='" + reservationList[j].roomId + "']").css('background-color', color[0]);
        //Second timeslot classtoggle=reservedd;
        if (reservationList[j].initialTimeslot === reservationList[j].finalTimeslot) {
            $("li[data-timeslot='" + (reservationList[j].initialTimeslot) + "']li[data-room='" + reservationList[j].roomId + "']").addClass("openSingleTimeslotReserve");
        }
        else{
            var time = "<u>Time</u>: From " + reservationList[j].initialTimeslot + " to " + (parseInt(reservationList[j].finalTimeslot) + 1);
            var description = "<u>Description</u>: " + reservationList[j].description;
            // var waitingList = "<u>Waiting List:</u>:";
            $("li[data-timeslot='" + (reservationList[j].initialTimeslot + 1) + "']li[data-room='" + reservationList[j].roomId + "']").html('<div class="reserved-left">' + time + "</br>" + description + "</br></div>");
        }
    }


};

$(".openSingleTimeslotReserve").hover(function () {


    $(".Single-Timeslot-Reserve").toggle(0);
    $(".Single-Timeslot-Reserve").css('opacity', '0');
    $(".Single-Timeslot-Reserve").position({
        my: "left top",
        at: "right+7 top+-7",
        of: $(this),
    });
    $(".Single-Timeslot-Reserve").toggle(0);
    $(".Single-Timeslot-Reserve").css('opacity', '1');
    $(".Single-Timeslot-Reserve").toggle(300);


},function(){
  $(".Single-Timeslot-Reserve").toggle(300);

});

var serverSession;
//Get reservation info from the server to populate the timeslots
$.connection.hub.start().done(function () {
    console.log('Connection established')
    var serverSession = $.connection.calendarHub;
    setCalendarDate();
    $(".getNext").click();
});

//Manually attach updateCalendar

//Jquery to update the timeslots


$("#submitButton").click(function () {
    $(".glyphicon-remove").click();
})

//Login popup
$(".dropdownLogin").click(function () {
    $(".login-popup").toggle();
    $(".login-popup").css('opacity', '0');
    $(".login-popup").css('width', '500px');
    $(".login-popup").toggle();
    $(".login-popup").css('opacity', '1');
    $(".login-popup").toggle(300);
});
$(".dropdownLogout").click(function () {
    $(".login-popup").toggle();
    $(".login-popup").css('opacity', '0');
    $(".login-popup").css('width', '200px');
    $(".login-popup").toggle();
    $(".login-popup").css('opacity', '1');
    $(".login-popup").toggle(300);
    $("#username").remove();
    $("#password").remove();
    $("#failedMessage").remove();
    $("#loginButton").html("Log Out");


});

function OnSuccess(data) {
    if (data.responseText != "Success") {
        $("#failedMessage").html("Invalid credentials");
    }
    else {
        location.reload(true);
    }

}
serverSession.client.populateReservations = function (reservationList) {
    $(".reservations .reservation-content ").empty();
    for(var i = 0; i<reservationList.length ; i++)
    {
        var resID = reservationList[i].reservationId;
        var des = reservationList[i].description;
        var firstTime = reservationList[i].initialTimeslot;
        var secondTime = reservationList[i].finalTimeslot;
        var roomID = reservationList[i].roomId;
        var date = reservationList[i].date;
        date = date.substr(0, 10);
        buildNewReservationItem(resID, des, firstTime, secondTime, roomID, date);
    }
}


$(".incomingMessage").on('click', function () {
    $(".messages").toggle(200);
    $(".incomingMessage").removeClass('active');
});
//Messages
serverSession.client.incomingMessage = function (message) {
    $(".incomingMessage").addClass('active');
    $(".messages").prepend('<div class="message-item">'+message+'</div>')
  
   

}
//adds a show waitlist button to timeslots with waitlists
//complexity can be heaviily improved but not enough time left
serverSession.client.updateWaitlist = function (timeslotList,reservationList,userCatalog) {
    var roomId;
    var userName;
    for (var i = 0; i < timeslotList.length; i++) {
        var waitlistUsers = "";
        if (timeslotList[i].waitlist.length != 0) {
            for (var k = 0; k < timeslotList[i].waitlist.length; k++) {
               for (var n = 0; n < userCatalog.length; n++) {
                    if(timeslotList[i].waitlist[k]==userCatalog[n].userID)
                    {
                        waitlistUsers = waitlistUsers +userCatalog[n].name+"-";
                    }
                }
            }
            for (var j = 0; j < reservationList.length; j++) {
                if(timeslotList[i].reservationID == reservationList[j].reservationId)
                {
                    roomId = reservationList[j].roomId;
                
                    break;
                }
            }
            $("li[data-timeslot='" + timeslotList[i].hour + "']li[data-room='" + roomId + "']").append('<div class="get-waitlist" data-users="'+waitlistUsers+'">' + timeslotList[i].waitlist.length + '</div>');
            
        }
    }

}
function buildNewReservationItem(reservationId, description, initialTimeSlot, finalTimeslot , roomID,date ) //reservtion id goes in .$(".cancelReservation).data(reservationId)
{
    var reservationItem = 
       '<div class="reservation-item"><div data-resid="' + reservationId + '" class="content-room">' + roomID + '</div><div class="content-date">' + date + '</div><div class="content-description">-' + description + '</div><div class="content-from">' + initialTimeSlot + '</div><div class="content-to">' + (parseInt(finalTimeslot)+1) + '</div></div>';

    $(".reservations .reservation-content ").append(reservationItem);
}


//Show reservations
$(".showReservations").click(function () {
    $(".reservations").toggle('fade',200);
    $(".modify-reservation").toggle('fade', 200);
    $(".showReservations").toggleClass('active');
    $(".reservationButton").click();
});


function notLoggedIn () {
    $(".dropdownLogin").click();
    $("#failedMessage").html("Sign in to continue")

};


function remakeCalendar() {

    var reservedTimeslots = $(".timeslots li[class]");
    for (var i = 0; i < reservedTimeslots.length ; i++) {
        $(reservedTimeslots[i]).html($(reservedTimeslots[i]).data('timeslot') + ':00')
    }
    $(".timeslots li").removeClass("reserved reserved-header active SingleTimeslotReserve");
    $(".timeslots li").removeAttr("style");

    

}
//Modify reservations form fill
$(".reservation-content").on('click',".reservation-item",function(){
    $(".reservation-item.active").toggleClass('active');
    
    $(this).toggleClass('active');
    $(".modify-buttons").show(50);
    var activeElement = $(".reservation-item.active");
    $("select[name='roomId']").val($(".reservation-item.active").find(".content-room").html());
    $("select[name='initialTimeslot']").val($(".reservation-item.active").find(".content-from").html().split(":")[0]);
    $("select[name='finalTimeslot']").val($(".reservation-item.active").find(".content-to").html().split(":")[0]);
    $("input[name='date']").val($(".reservation-item.active").find(".content-date").html());
    $("input[name='description']").val($(".reservation-item.active").find(".content-description").html());
    $("input[name='resid']").attr("value", $(".reservation-item.active").find(".content-room").data('resid'));
    setCalendarDate();

    ddl_finalTimeslot_restriction();

});
    
$(".deleteReservation").on('click', function () {

    $("input[name='resid']").attr("value", $(".reservation-item.active").find(".content-room").data('resid'));
        setCalendarDate();
        $(".cancelReservationAjax").click();
    });
       
    
$(".reservation-content").on('click', ".modifyReservation", function () {
    
    $(".modify-reservation").toggle(300);

});


$(".reservation-tab").click(function () {
    if ($(".reservation-tab").hasClass("active")) { }
    else {
        $(".reservation-tab").toggleClass('active');
        $(".waitlist-tab").toggleClass('active');
        $(".reservation-content").toggle('active');
        $(".waitlist-content").toggle('active');
    }
});


$(".waitlist-tab").click(function () {
    if ($(".waitlist-tab").hasClass("active")) { }
    else {
        $(".waitlist-tab").toggleClass('active');
        $(".reservation-tab").toggleClass('active');
        $(".reservation-content").toggle('active');
        $(".waitlist-content").toggle('active');

    }
});

function ddl_finalTimeslot_restriction() {
    $(".ddl-finalTimeslot").children().hide();
    var option = parseInt($('.ddl-initialTimeslot option:selected').val());
    for (var i = option + 1; i <= option + 4 ; i++) {
        $(".ddl-finalTimeslot").children("option[value=" + i + "]").show();
    }
}
ddl_finalTimeslot_restriction();

$(".ddl-initialTimeslot").change(function () {

    $(".ddl-finalTimeslot").children().hide();

    var option = parseInt($('.ddl-initialTimeslot option:selected').val());
    if (option > $(".ddl-finalTimeslot").val()) {
        $(".ddl-finalTimeslot").val(option);
    }
    else if (option < $(".ddl-finalTimeslot").val() + 4) {
        $(".ddl-finalTimeslot").val(option + 4);
    }

    for (var i = option+1; i <= option + 4 ; i++) {
        $(".ddl-finalTimeslot").children("option[value=" + i + "]").show();
    }
});

//get-waitlist click functionailty
$(".timeslots li ul li").on('mouseenter','.get-waitlist', function (event) {
    var username = $(this).data('users').split("-");
    $(".waitlist-tooltip").empty();
    $(".waitlist-tooltip").toggle(0);
    $(".waitlist-tooltip").css('opacity', '0');
    for (var i = 0; i < username.length-1; i++)
    {
        $(".waitlist-tooltip").append('<div style="waitlist-item">'+username[i]+'</div>');
    }
    $(".waitlist-tooltip").position({
        my: "left top",
        at: "right+7 top+-7",
        of: $(this)
    });
    $(".waitlist-tooltip").toggle(0);
    $(".waitlist-tooltip").css('opacity', '1');
    $(".waitlist-tooltip").toggle('fade',300);

});
$(".timeslots li ul li").on('mouseleave', '.get-waitlist', function (event) {
    event.stopPropagation();
    $(".waitlist-tooltip").toggle('fade', 100);
    

});



//Populate waitlist functionailty
function populateWaitlist() {


}