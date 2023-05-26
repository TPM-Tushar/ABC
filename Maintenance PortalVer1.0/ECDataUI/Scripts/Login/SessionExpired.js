
//* Project Id    :
//    * Project Name  :   GAURI
//    * Name          :   SessionExpired.js        
//    * Description   :    .
//    * Author        :  
//    * Creation Date :   
//    * Modified By   :  
$(document).ready(function () {
    if ($("#ajaxexpiry").val() == "True")
    {
        $.unblockUI();
        window.location.href = "/Error/SessionExpire";
    }
   
    var maskHeight = $(document).height();
    var maskWidth = $(window).width();
    $('#mask').css({ 'width': maskWidth, 'height': maskHeight });
    $('#mask').fadeIn(500);
    $('#mask').fadeTo("slow", 0.8);
    var winH = $(window).height();
    var winW = $(window).width();
    $("#dialog").css('top', winH / 2 - $("#dialog").height() / 2);
    $("#dialog").css('left', winW / 2 - $("#dialog").width() / 2);
    $("#dialog").fadeIn(500);
});