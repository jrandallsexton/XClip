
//var URL_REST_SVC = 'http://dirty.lazycodeslinger.com/dirtyRestSvc';
var URL_REST_SVC = 'http://vesuvius/dirtyRestSvc';

var numQuestions = null;
var inReview = false;
var settings = null;

var index = 0;

var images = [];
var captions = [];
var rate = [];
var answerIndex = 0;

//http://vesuvius/dirty/viewer.html

function onCutSelected(id) {

	$( '.list-group-item' ).removeClass('btn-primary');
	$( '#li' + id ).addClass('btn-primary');

	var cut = cuts[id];
	$( '#in' ).val( cut.Start );
	$( '#out' ).val( cut.Stop );

}

function initTaggerHtml() {
	loadMediaToTag();
}

function endsWith(str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
}

function loadMediaToTag() {

	var imgTagInfo = ajaxGet(URL_REST_SVC + '/imageUntagged');
	var remain = ajaxGet(URL_REST_SVC + '/tagcountRemaining');

	var html = [];

	if (endsWith(imgTagInfo.url, ".mp4")) {
		html.push('<div style="display:inline;float:left;"><video style="display:inline;max-height:800px;max-width:600px;" controls>');
		html.push('<source src="' + imgTagInfo.url + '" type="video/mp4"></video></div><div style="width:25%;float:left;">');
	}
	else {
		html.push('<img src="' + imgTagInfo.url + '" style="width:50%;float:left;"/><div style="width:25%;float:left;padding-left:10px;">');		
	}

	if (imgTagInfo.previouslyTagged) {
		var tags = ajaxGet(URL_REST_SVC + '/tags');
		$.each(tags, function(index,item) {
			html.push('<input id="' + item.id + '" type="button" value="' + item.text + '" onclick="onTagged(\'' + imgTagInfo.id + '\', this.id)"></input>');
		});
	}

	html.push('<br/><br/>');
    html.push('<input id="txtVal" type="text" data-theme="a" value="100"/>');
    html.push('<label for="txtVal" style="padding-left:10px;">Rating</label>');
	html.push('<br/><br/>');
    html.push('<input id="checkbox1" name="chkToDo" type="checkbox" data-theme="a"/>');
    html.push('<label for="checkbox1">ToDo!</label>');
    html.push('<input id="checkbox2" name="chkOMG" type="checkbox" data-theme="a"/>');
    html.push('<label for="checkbox2">OMG!!</label>');
    html.push('<input id="checkbox3" name="chkVideo" type="checkbox" data-theme="a"/>');
    html.push('<label for="checkbox3">Video!</label>');

	html.push('<textarea id="txtWhy" rows="4" cols="50"></textarea><br/><br/>');
	html.push('<input type="button" value="DO NOT USE" onclick="onDeleteImage(\'' + imgTagInfo.id + '\')"></input>');
	html.push('<input type="button" value="DEFER" onclick="onDeferImage(\'' + imgTagInfo.id + '\')"></input>');

	if (imgTagInfo.previouslyTagged) {
		html.push('<input id="btnTag" type="button" value="DONE TAGGING" onclick="onTagComplete(\'' + imgTagInfo.id + '\')"></input><br/><br/>');
	}
	else {
		html.push('<input id="btnTag" type="button" style="display:none;" value="DONE TAGGING" onclick="onTagComplete(\'' + imgTagInfo.id + '\')"></input><br/><br/>');		
	}
	
	html.push('<label>' + remain + ' left to tag</label><br/><br/>');
	html.push('<label>' + imgTagInfo.id + '</label></div>');
	$('#container').html(html.join(''));
}

function onTagComplete(imgId) {

	var imgComplete = new Object();
	imgComplete.imageId = imgId;
	imgComplete.comment = $('#txtWhy').val();
	imgComplete.assignee = '5CC564ED-8A49-41BE-B748-8F528864D4EE';
    imgComplete.value = parseFloat($('#txtVal').val());
    imgComplete.addToList = ($('#checkbox1:checked').val() !== undefined);
    imgComplete.omg = ($('#checkbox2:checked').val() !== undefined);
    imgComplete.video = ($('#checkbox3:checked').val() !== undefined);

	var completed = ajaxPost(URL_REST_SVC + '/imageTagComplete', JSON.stringify(imgComplete));
	if (completed) {
		loadMediaToTag();
	}
	else {
		alert('error completing tagging media');
	}	
}

function onTagged(imageId, tagId) {
	var imgTag = new Object();
	imgTag.imageId = imageId;
	imgTag.tagId = tagId;
	ajaxPost(URL_REST_SVC + '/tagImage', JSON.stringify(imgTag));
	$('#' + tagId).hide();	
	$('#btnTag').show();
}

function onDeferImage(id) {
	var deferred = ajaxGet(URL_REST_SVC + '/imageTagDefer/' + id);
	if (deferred) {
		loadMediaToTag();
	}
	else {
		alert('error deferring media');
	}	
}

function onDeleteImage(id) {
	var deleted = ajaxGet(URL_REST_SVC + '/imageDelete/' + id);
	if (deleted) {
		loadMediaToTag();
	}
	else {
		alert('error deleting media');
	}
}

function initViewerHtml() {

	var html = [];

	var files = new Array();
	files.push('37');
	files.push('38');
	files.push('39');

	$.each(files, function(index,item) {	
	html.push('<video width="320" height="240" style="margin:0px; border:0px; padding:0px;" autoplay loop tabindex="' + index + '">');	
	  	html.push('<source src="media/vid/' + item + '.webmhd.webm" type=\'video/webm; codecs="vp8, vorbis"\' />');
	html.push('</video>');
	});

	$('#content').html(html.join(''));

}

function initDefaultHtml() {
    $.allowCrossDomainPages = true;
    //slideShow();
    //return;
	var settings = getSettings();
	
	if (settings.denied) {
		$('#content').html('<p>Denied</p>');
	}
	else if (settings.completed) {
		$('#content').html('<p>Previously Completed</p>');
	}
	else {
		validateSecurity();
	}	
}

function validated(){
	var html = [];
	html.push('<div id="accordion">');
	html.push('<h3><a href="#">Foreward / Disclaimer</a></h3>');
	html.push('<div>' + '<p>' + getIntro() + '</p>');

	html.push('<input type="button" style="float: left;" value="Get me the hell out of here" class="ui-widget ui-corner-all" onclick="denied()" />');
	html.push('<input type="button" style="float: right;" value="Ok, sure" class="ui-widget ui-corner-all" onclick="startQuestions()" />');
	html.push('</div>');

	$( '#content' ).html(html.join(''));
	$( "#accordion" ).accordion();
	$('html, body').animate({ scrollTop: 0 }, 0);
}

function showPlease(){
	$('#messageDialog').dialog('destroy');
	$("#messageDialog").html(getPlease());
	$('#messageDialog').dialog({
		width: 450,
		height: 'auto',
		modal: true,
		autoOpen: false,
		title: 'Remember ...',
		open:function() {
			$(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
			},
		buttons: {
			Ok: function() {
				$(this).dialog('close');
				$('html, body').animate({ scrollTop: 0 }, 0);
			}
		}
	});

	$('#messageDialog').dialog('open');	
}

function validateSecurity(){

	$('#dialog-form').dialog('destroy');
	$('#txtDate').val('the honest pint');
	$('#dialog-form').dialog({
		width: 450,
		height: 'auto',
		modal: true,
		autoOpen: false,
		title: 'Security Question',
		open:function() {
			$(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
			},
		buttons: {
			Cancel: function() {
				$(this).dialog('close');
			},
			Submit: function() {
				if (securityAnswerIsValid($('#txtDate').val())) {
					validated();
					$(this).dialog('close');			
				}
				else {
					alert('Invalid');
					$(this).dialog('close');
				}
			}
		}
	});

	$('#dialog-form').dialog('open');
}

function nextAnswer() {
	    var item = ajaxGet(URL_REST_SVC + '/todoItemNext/' + answerIndex);
	answerIndex++;
    var html = [];

    if (item.isQuestion) {
        html.push('<label>' + item.mediaSrc + '</label><br/><br/>');
    }
    else {
        html.push('<img style="width: 400px; height: auto;" src="' + item.mediaSrc + '"/><br/><br/>');
    }
    html.push('<label>Rating: ' + item.value + '</label><br/><br/>');
    html.push('<label>ToDo:   ' + item.toDo + '</label><br/><br/>');
    html.push('<label>OMG:    ' + item.omg + '</label><br/><br/>');
    html.push('<label>Video:  ' + item.vid + '</label><br/><br/>');
    html.push('<label>Comment: ' + item.comment + '</label>');
    //alert(html.join(''));
    $('#content').html(html.join(''));
}

function initAnswerHtml(){
    var item = ajaxGet(URL_REST_SVC + '/todoItemNext/' + answerIndex);
	answerIndex++;
    var html = [];

    if (item.isQuestion) {
        html.push('<label>' + item.mediaSrc + '</label><br/><br/>');
    }
    else {
        html.push('<img style="width: 400px; height: auto;" src="' + item.mediaSrc + '"/><br/><br/>');
    }
    html.push('<label>Rating: ' + item.value + '</label><br/><br/>');
    html.push('<label>ToDo:   ' + item.toDo + '</label><br/><br/>');
    html.push('<label>OMG:    ' + item.omg + '</label><br/><br/>');
    html.push('<label>Video:  ' + item.vid + '</label><br/><br/>');
    html.push('<label>Comment: ' + item.comment + '</label>');
    //alert(html.join(''));
    $('#content').html(html.join(''));
	//var answers = getAnswers();

	//$('#content').html('<p class="answers">' + answers + '</p><br/><br/><p>' + getAnalysis() + '</p><br/><p>' + getAnalysisHer() + '</p>');
}

function startQuestions(){

	var settings = getSettings();
	
	if (settings.denied) {
		$('#content').html('<p>You previously denied the questions.</p>');
		return;
	}

	var html = [];
	html.push('<label>You can change your answers prior to submitting.  Ignore the black star for rating; it is there to clear your answer if you need to.</label></br></br>');
	html.push('<table>');

	html.push('<tr>');
	html.push('<td><img src="img/star-gold32.png"/></td><td/><td/><td/><td/><td><label class="rating">WTF is wrong with you?</label></td>');
	html.push('</tr>');
	
	html.push('<tr>');
	html.push('<td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td/><td/><td/><td><label class="rating">I don\'t think so / I\'d really rather not.</label></td>');
	html.push('</tr>');
	
	html.push('<tr>');	
	html.push('<td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td></td><td/><td/></td><td><label class="rating">Maybe.</label></td>');
	html.push('</tr>');
	
	html.push('<tr>');	
	html.push('<td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td/><td><label class="rating">Sure, no problem. :-)</label></td>');
	html.push('</tr>');
	
	html.push('<tr>');	
	html.push('<td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td><img src="img/star-gold32.png"/></td><td><label class="rating">I\'d love to do that!  That\'s hot!!!</label></td>');
	html.push('</tr>');
		
	html.push('</table>');

	html.push('</br>');
	html.push('<div id="accordion">');

	var questions = getQuestions();
	var max = questions.length;
	numQuestions = max;

	$.each(questions, function(index, item) {
		html.push('<h3 id="h' + index + '"><a href="#">#' + (parseInt(item.order) + parseInt(1)) + '</a></h3>');
		html.push('<div id="d' + index + '">' + item.text + '</br></br>');
		html.push('<div id="rateit' + index + '" class="rateit bigstars" data-rateit-starwidth="32" data-rateit-starheight="32"></div>');
		if (index < (max - 1)) {
			html.push('</br></br><input type="button" id="btnNext' + index + '" value="Next" class="ui-widget ui-corner-all" onclick="onnext(' + (index) + ')" />');
		}
		else {
			html.push('</br></br><input type="button" id="btnNext' + index + '" value="Review All Answers" class="ui-widget ui-corner-all" onclick="onreview()" />');			
		}
		html.push('</div>');
	});

	html.push('</div>');

	$('#content').html(html.join(''));
	$( "#accordion" ).accordion();
	$( ".rateit" ).rateit();

	showPlease();
		//$("#rateit" + index).bind('rated', function (event, value) { $('#value' + index).text('You\'ve rated it: ' + value); });
		//$("#rateit" + index).bind('over', function (event, value) { $('#hover' + index).text('Hovering over: ' + value); });
}

function onTag(isCorrect, tag){
	var tagResponse = new Object();
	tagResponse.correct = isCorrect;
	tagResponse.tag = tag;
	ajaxPost(URL_REST_SVC + '/tagresponse/' + tag, JSON.stringify(tagResponse));
	$('#div' + tag).hide();
}

function securityAnswerIsValid(answer){
	return ajaxGet(URL_REST_SVC + '/securityValidated/' + answer);
}

function getIntro(){
	return ajaxGet(URL_REST_SVC + '/intro');	
}

function getSettings(){
	return ajaxGet(URL_REST_SVC + '/settings');
}

function onreview(){
	if (!inReview) {
		inReview = true;
		for (var i=0; i<(numQuestions - 1); i++) {
			$('#btnNext' + i).prop('value', 'Confirm');
			$('#d' + i).fadeIn('fast');
			$('#h' + i).fadeIn('fast');
		}
		$('#btnNext' + (numQuestions-1)).prop('value', 'Submit');
		$( "#accordion" ).accordion( "option", "active", 0 );
	}
	else {
		completed();
		var results = getAnalysisHer();
		$('#content').html('<p>' + getResult() + '</p><label>Here\'s my analysis.  Tell me if I\'m correct or not.</label><br/><br/>' + results + '<br/><br/><p>Thanks for playing!!! :-)</p>');
	}
}

function onnext(index){

	var answer = new Object();
	answer.value = $('#rateit' + index).rateit('value');
	answer.comment = '';

	if (answer.value == 0) {
		showMessage('Not Rated', 'You didn\'t select an answer.  If you really wanted to give it a zero, just select 1 star and add a comment during the review', true, 400, true);
		return;
	}

	if (inReview && (answer.value < 4)) {
		$('#dialogExplain').dialog('destroy');
		$('#txtReason').val('');
		$('#dialogExplain').dialog({
			width: 450,
			height: 'auto',
			modal: true,
			autoOpen: false,
			title: 'Explanation?',
			open:function() {
				$(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
				},
			buttons: {
				'No comment': function() {
					ajaxPost(URL_REST_SVC + '/answer/' + index, JSON.stringify(answer));
					$( "#accordion" ).accordion( "option", "active", parseInt(index) + 1 );
					$('#d' + index).fadeOut('fast');
					$('#h' + index).fadeOut('fast');
					$(this).dialog('close');
				},
				'Add Comment': function() {
					answer.comment = $('#txtReason').val();
					ajaxPost(URL_REST_SVC + '/answer/' + index, JSON.stringify(answer));
					$( "#accordion" ).accordion( "option", "active", parseInt(index) + 1 );
					$('#d' + index).fadeOut('fast');
					$('#h' + index).fadeOut('fast');
					$(this).dialog('close');
				}
			}
		});
	
		$('#dialogExplain').dialog('open');
	}
	else {
		ajaxPost(URL_REST_SVC + '/answer/' + index, JSON.stringify(answer));
		$( "#accordion" ).accordion( "option", "active", parseInt(index) + 1 );
		$('#d' + index).fadeOut('fast');
		$('#h' + index).fadeOut('fast');
	}	
}

function getAnalysisHer(){
	return ajaxGet(URL_REST_SVC + '/analysisHer');	
}

function getAnalysis(){
	return ajaxGet(URL_REST_SVC + '/analysis');	
}

function getResult(){
	return ajaxGet(URL_REST_SVC + '/result');	
}

function getAnswers() {
	return ajaxGet(URL_REST_SVC + '/answers');
}

function getPlease(){
	return ajaxGet(URL_REST_SVC + '/please');	
}

function getQuestions() {
	return ajaxGet(URL_REST_SVC + '/questions');
}

function getTags(questionIndex){
	return ajaxGet(URL_REST_SVC + '/tags/' + questionIndex);	
}

function denied() {
	ajaxGet(URL_REST_SVC + '/denied');
	window.location.href = 'http://www.google.com';
}

function completed() {
	ajaxGet(URL_REST_SVC + '/completed');
	$('#content').html('<p>Thanks for playing!!! :-)</p>');	
}

function addImage(file, caption, allowRate) {
    images[index] = file;
    captions[index] = caption;
    rate[index] = allowRate;
    index++;
}

function slideShow() {

    addImage('touch00.gif', 'Love to watch you touching yourself ...', false);
    addImage('touch01.gif', 'All wet ... just like this ...', false);
    addImage('touch02.gif', '... rubbing your clit ... wow!', false);
    addImage('facesitting.gif', 'I know this is girl-on-girl, but I think you get the point.', false);
    addImage('eatingAss.gif', 'Once again, girl-on-girl, but you know I want to!', false);
    addImage('cs00.gif', 'I\'d love to blast all over you like this!', true);
    addImage('pw00.gif', 'But not before I do this ...', true);
    addImage('pw01.gif', 'And this!  I WANT you to use me!', true);
    addImage('pw02.gif', 'And of course running my tongue all over you!', true);
    addImage('pw03.gif', 'And more!', true);
    addImage('eatingAss02.gif', 'And you know I really want to do this!', true);
    addImage('cw00.gif', 'But after I please you, I want some attention too!', true);
    addImage('cw01.gif', 'Like this!', true);
    addImage('cw02.gif', 'Or this! (ahem!  just kidding; you\'re enough!)', true);
    addImage('cw03.gif', 'Or this, yes please! Wow!', true);
    addImage('cw04.gif', 'Wow!', true);
    addImage('couch.gif', 'We could do it on the couch (again) ...', true);
    addImage('shower.gif', '... or maybe in the shower!', true);
    addImage('elevator.gif', '... or an elevator?', true);
    addImage('kitchen.gif', '... or an maybe in the kitchen?', true);
    addImage('stairs.gif', 'What about on the stairs?  Just kidding.  That does NOT look comfortable!', true);
    addImage('odd.gif', '... or perhaps something just a bit odd?', true);
    addImage('odd01.gif', 'But not THIS odd!', true);
    addImage('oil00.gif', 'Maybe I could oil you up ...', true);
    addImage('oil01.gif', '... just like this ...', true);
    addImage('oil02.gif', '... or this ...', true);
    addImage('oil03.gif', '... or this ...', true);
    addImage('analOiled.gif', '... then slide my cock in your ass.', true);
    addImage('analPlay00.gif', 'Perhaps I should warm you up before that though ...', true);
    addImage('spank.gif', '... maybe a good spanking?', true);
    addImage('slow.gif', 'We could do it nice and slow ...', true);
    addImage('submissive00.gif', '... or perhaps after a spanking you should be made to obey ...', true);
    addImage('submissive01.gif', '... and keep doing whatever I need.', true);
    addImage('ride.gif', 'Maybe you could ride my cock for awhile ...', true);
    addImage('ride02.gif', '... perhaps like this?', true);
    addImage('assCum00.gif', 'Maybe I could bend you over ... I wouldn\'t last long though!', true);
    addImage('assCum01.gif', 'I\'d love to cum all over your ass just like this!', true);
    addImage('assCum02.gif', 'Or this!', true);
    addImage('cs01.gif', 'I\'d love for you to finish me off like this ...', true);
    addImage('cs02.gif', '... or this ...', true);
    //addImage('cs03.gif', '... or this ...');
    addImage('cs04.gif', '... or this ...', true);
    addImage('cs05.gif', '... or this ...', true);
    addImage('cs06.gif', '... or this ...', true);
    addImage('cs07.gif', '... or this ...', true);
    //addImage('cs08.gif', '... or this ...');
    addImage('cs09.gif', '... or this ...', true);
    addImage('cs10.gif', '... or this ...', true);
    addImage('cs11.gif', '... or this ...', true);
    addImage('cs12.gif', '... or this ...', true);
    addImage('cs13.gif', '... or this ...', true);
    addImage('cs14.gif', '... or this ...', true);
    addImage('cs15.gif', '... or this ...', true);
    addImage('cs16.gif', '... or this ...', true);
    addImage('cs17.gif', '... or this ...', true);
    addImage('cs18.gif', '... or this ...', true);
    addImage('cs19.gif', '... or this ...', true);
    addImage('cs20.gif', '... or this ...', true);
    addImage('cs21.gif', '... or this ...', true);
    addImage('cs22.gif', '... or this ...', true);
    addImage('cs23.gif', '... or this ...', true);
    //addImage('cs24.gif', '... or this ...');
    //addImage('cs25.gif', '... or this ...');
    //addImage('cs26.gif', '... or this ...');
    addImage('selfFinish00.gif', 'Or I could finish the job myself ...', true);
    addImage('selfFinish01.gif', '... like this ...', true);
    addImage('selfFinish02.gif', '... or this ...', true);
    addImage('selfFinish03.gif', '... or this ...', true);
    addImage('selfFinish04.gif', '... or this ...', true);
    addImage('selfFinish05.gif', '... or this ...', true);
    addImage('selfFinish06.gif', '... or this ...', true);
    addImage('selfFinish07.gif', '... or this ...', true);
    addImage('selfFinish08.gif', '... or this ...', true);
    addImage('selfFinish09.gif', '... or this ...', true);
    addImage('selfFinish10.gif', '... or this ...', true);
    addImage('selfFinish11.gif', '... or this ...', true);
    addImage('selfFinish12.gif', '... or this ...', true);
    addImage('selfFinish13.gif', '... or this ...', true);
    addImage('selfFinish14.gif', '... or this ...', true);
    addImage('selfFinish15.gif', '... or this ...', true);
    addImage('selfFinish16.gif', '... or this ...', true);
    addImage('selfFinish17.gif', '... or this ...', true);
    addImage('selfFinish18.gif', '... or this ...', true);
    
    index = 0;

    var html = [];
    html.push('<div style="text-align:center; vertical-align:middle;"><img src="media/' + images[index] + '" /></div>');

    $('#messageDialog').dialog('destroy');
    $("#messageDialog").html(html.join(''));
    $('#messageDialog').dialog({
        width: 700,
        height: 500,
        modal: true,
        autoOpen: false,
        title: captions[index],
        open: function () {
            $(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
        },
        buttons: {
            '<--': function () {
                index--;
                $(this).html('<label>' + captions[index] + '</label><br/><br/><div style="text-align:center"><img src="media/' + images[index] + '"/></div>');
                $(this).dialog("option", "title", images[index]);
            },
            'No Comment': function () {
                index--;
                $(this).html('<label>' + captions[index] + '</label><br/><br/><div style="text-align:center"><img src="media/' + images[index] + '"/></div>');
                $(this).dialog("option", "title", images[index]);
            },
            '!!! Gross !!!': function () {
                index++;
                $(this).html('<label>' + captions[index] + '</label><br/><br/><div style="text-align:center"><img src="media/' + images[index] + '"/></div>');
                $(this).dialog("option", "title", images[index]);
            },
            'Not Hot': function () {
                index++;
                $(this).html('<label>' + captions[index] + '</label><br/><br/><div style="text-align:center"><img src="media/' + images[index] + '"/></div>');
                $(this).dialog("option", "title", images[index]);
            },
            'Hot!': function () {
                index++;
                $(this).html('<label>' + captions[index] + '</label><br/><br/><div style="text-align:center"><img src="media/' + images[index] + '"/></div>');
                $(this).dialog("option", "title", images[index]);
            },
            '-->': function () {

                index++;

                $(this).html('<div style="text-align:center; vertical-align:middle;"><img src="media/' + images[index] + '" /></div>');
                $(this).dialog("option", "title", captions[index]);

                if (index == 28) {
	                $(this).dialog("option", "height", 575);
                }
                else {
 	                $(this).dialog("option", "height", 500);               	
                }

				if (index > 0) {
					$('.ui-button:contains(<--)').show();	
				}
				else {
					$('.ui-button:contains(<--)').hide();	
				}

				if (rate[index] == false) {
					$('.ui-button:contains(Comment)').hide();
					$('.ui-button:contains(Gross)').hide();
					$('.ui-button:contains(Hot)').hide();
				}
				else {
					$('.ui-button:contains(Comment)').show();
					$('.ui-button:contains(Gross)').show();
					$('.ui-button:contains(Hot)').show();
				}
            }
        }
    });

    $('#messageDialog').dialog('open');

	if (index == 0) {
		$('.ui-button:contains(<--)').hide();		
	}
	else {
		$('.ui-button:contains(<--)').show();		
	}	

	if (rate[index] == false) {
		$('.ui-button:contains(Comment)').hide();
		$('.ui-button:contains(Gross)').hide();
		$('.ui-button:contains(Hot)').hide();
	}
	else {
		$('.ui-button:contains(Comment)').show();
		$('.ui-button:contains(Gross)').show();
		$('.ui-button:contains(Hot)').show();		
	}
}