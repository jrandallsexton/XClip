
var NULL_GUID = '00000000-0000-0000-0000-000000000000';
var videoPlayer = null;

var _mediaSource = null; // mediaSource.js
var _batch = null;

var _tags = [];

var _tagsVidIds = [];
var _tagsClipIds = [];

var cuts = [];
var tagIdxVal = [];
var _userId = 1;
var _collectionId = 2;

function initHtml() {

	apiWrapper.getRandomMedia(_collectionId, function(data) {

        _mediaSource = new mediaSource(data);
        _batch = new batch();
        _batch.sourceId = _mediaSource.uId;

        initVideoPlayer(_mediaSource, function() {

            $( ".fine" ).on( "click", function() {
                $( ".ctl" ).hide();
                var skip = parseFloat( $(this).attr('data-value') );
                setPlayerPos(skip);
            });

            $( '#fname').html( _mediaSource.fName );

            apiWrapper.getTags(_userId, _collectionId, function(data) {

                _tags = data;
                tagIdxVal = [];

                $.each(_tags, function(index, item) { tagIdxVal[item.id] = item.text; });

                $("#clipTag").select2(
                {
                    tags: _tags,
                    dropdownCssClass: "bigdrop"
                }
                ).on("change", function(e) {
                    if ((e.added != null) && (e.added != undefined)) {
                        // was it a new value?  if e.added.id is not an integer, it is text: that means a new tag
                        if (isNaN(e.added.id)) {
							apiWrapper.tagSave(e.added.id, function(result) {
								debugger;
								_tagsClipIds.push(result);
							});
						}
						else {
							_tagsClipIds.push(e.added.id);
						}
                    }
                });

            });

            apiWrapper.getVideoTags(_mediaSource.uId, function(data) {

                $("#vidTag").select2(
                {
                    tags: _tags,
                    dropdownCssClass: "bigdrop"
                }
                ).on("change", function(e) {
                    if ((e.added != null) && (e.added != undefined)) {
						// was it a new value?  if e.added.id is not an integer, it is text: that means a new tag
						if (isNaN(e.added.id)) {
							var tagCreationModel = new TagCreationModel(_userId, _collectionId, e.added.id);
							apiWrapper.tagSave(tagCreationModel, function(result) {
								_tagsVidIds.push(result);
							});
						}
						else {
							_tagsVidIds.push(e.added.id);
						}
                    }
                });
                $('#vidTag').select2('data', data);
            });

        });
    });
	//$( ".chk" ).on( "click", function() { onTagCheckChanged(this); });
}

function initVideoPlayer(mediaSource, callback) {
    var html = [];

    html.push('<source src="mediaSources/' + _collectionId + '/' + _mediaSource.uId + _mediaSource.fExt + '" type="' + _mediaSource.mimeType + '">');
    $( "#videoPlayer" ).html( html.join('') );

    videoPlayer = document.getElementById('videoPlayer');

    $( videoPlayer ).on( "seeked", onSeekEnd );

    $( '#in' ).val( null );
    $( '#out' ).val( null );

    callback();
}

function getNewVideo() {

	apiWrapper.getRandomMedia(_collectionId, function(data) {
		_mediaSource = data;

		_batch = new batch(null);
		_batch.sourceId = _mediaSource.uId;

		var html = [];
		html.push('<source src="mediaSources/2/' + _mediaSource.uId + _mediaSource.fExt + '" type="' + _mediaSource.mimeType + '">');
		$( videoPlayer ).html( html.join('') );

		// http://stackoverflow.com/questions/14799172/how-to-change-source-in-html5-video-with-jquery
		$( videoPlayer ).load();

		$( '#fname').html( _mediaSource.fName );

		$( '#in' ).val( null );
		$( '#out' ).val( null );

		$( '#lstCuts' ).html( '' );

		apiWrapper.getVideoTags(_mediaSource.uId, function(data) {

			$("#vidTag").select2(
			{
				tags: _tags,
				dropdownCssClass: "bigdrop"
			}).on("change", function(e) {
				if ((e.added != null) && (e.added != undefined)) {
					// was it a new value?  if e.added.id is not an integer, it is text: that means a new tag
					//if (isNaN(e.added.id)) { alert('new tag'); }
					debugger;
					_tagsVidIds.push(e.added.id);
				}
			});
			$('#vidTag').select2('data', data);
		});

		//$( ".chk" ).on( "click", function() { onTagCheckChanged(this); });
	});
}

function onTagCheckChanged(sender) {

	_tagIds.push(sender.value);
	/*var currentTagValue = tagIdxVal[sender.value];

	if ((currentTagValue == null) || (currentTagValue == undefined)) {
		tagIdxVal[sender.value] = true;
	}
	else {
		tagIdxVal[sender.value]	= !(tagIdxVal[sender.value]);
	}*/

}

function loadTags(targetControl) {

	tagIdxVal = [];

	var html = [];
	html.push( '<br/><table table-striped>' );

	var idx = 0;
	var maxCol = 7;

	$.each(tags(), function(index, item) {

		tagIdxVal[item.id] = item.text;

		if (idx == 0) {
			html.push( '<tr><td><input class="chk" type="checkbox" value="' + item.id + '"> ' + item.text + '</td>' );
			idx++;
		}
		else if (idx == maxCol) {
			html.push( '<td><input class="chk" type="checkbox" value="' + item.id + '"> ' + item.text + '</td></tr>' );
			idx = 0;
		}
		else {
			html.push( '<td><input class="chk" type="checkbox" value="' + item.id + '"> ' + item.text + '</td>' );
			idx++;
		}

	});

	html.push( '</table><br/>' );

	$('#' + targetControl).html( html.join('') );

}

function loadVideo() { return mediaSourceGetRandom(_collectionId); }

function onPlayPause() {

	if (videoPlayer.paused) {
		$( '#btnPlayPause' ).html('Pause <span class="glyphicon glyphicon-pause"></span>');
		videoPlayer.play();
	}
	else {
		$( '#btnPlayPause' ).html('Play <span class="glyphicon glyphicon-play"></span>');
		videoPlayer.pause();
	}
	
}

function onSeekEnd() { $( ".ctl" ).show(); }

function onSetStart() {
    $( '#in' ).val( videoPlayer.currentTime );
}

function onSetEnd() {
    $( '#out' ).val( videoPlayer.currentTime );
}

function addBatchItem() {

    var batchItem = new BatchItem(null);
	batchItem.id = NULL_GUID;
	batchItem.index = _batch.items.length;
	batchItem.start = $( '#in' ).val();
	batchItem.stop = $( '#out' ).val();
	batchItem.duration = '';
	batchItem.tags = _tagsClipIds;

	_batch.items.push( batchItem );

	var index = _batch.items.length;

	var tmp = (index + 1) + '. ' + batchItem.start + ' --> ' + batchItem.stop;

	if (batchItem.tags.length > 0) { tmp += ' : Tags --> '; }

	$.each(batchItem.tags, function(index, item) { tmp += tagIdxVal[item] + ' '; });

	$( '#lstCuts' ).append( '<li id="li' + index + '" onclick="onCutSelected(' + index +')" class="list-group-item">' + tmp + '</li>' );

	$( '#in' ).val( null );
	$( '#out' ).val( null );

    _tagsClipIds = new Array();

}

function saveBatch() {
	_batch.tags = _tagsVidIds;
	apiWrapper.batchSave(_batch, function(result) {
		if (result) {
			bootbox.alert("Batch saved successfully.<br/>The next video will now load.", function() {
				_tagsVidIds = new Array();
				_tagsClipIds = new Array();
				getNewVideo();
			});
		}
		else {
			alert('save error');
		}
	});
}

function skipSource() {
    apiWrapper.skipMediaSource(_mediaSource.uId, function(result) {
        if (result) {
            getNewVideo();
        }
        else {
            alert('skip error');
        }
    });
}

function deleteSource() {
    apiWrapper.mediaSourceDelete(_mediaSource.uId, function(result) {
        if (result) {
            getNewVideo();
        }
        else {
            alert('deletion error');
        }
    });
}

function setPlayerPos(val) {
	var currentPos = parseFloat(videoPlayer.currentTime);
	currentPos = currentPos + val;
	videoPlayer.currentTime = currentPos.toString();	
}