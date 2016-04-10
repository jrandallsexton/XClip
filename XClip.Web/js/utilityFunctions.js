
var ASSET_FILTERS = null; // format of [Property.Id]:[Property.Value]
var FILTERED_ASSET_TYPE_ID = null;
var FILTERED_ASSET_IS_INSTANCE = false;

var includeChildAssetTypes = true;

var propertyValues_selectHtml = '<select id="propertyValues" disabled="disabled" style="width:100%;" onChange="selectedAttributeValueChanged(this)" class="ui-widget ui-state-default">';
var propertyValues_inputHtml = '<input id="propertyValues" type="text" style="width:95%;" class="ui-widget ui-state-default"/>';

function activateHighlighting() {

	$('input').focus(function() {
		$(this).addClass('highlight');
	});

	$('input').blur(function(){
		$(this).removeClass('highlight');
	});

}

function activateRunAsAutoComplete(){

	$("#lstUsers").focusin(function(){

		if (this.autoCompleteRendered != true)
		{
			
			//turn on a load image?
			var imgRandomID = new Date().getTime();
			$(this).after("<img id='img" + imgRandomID +
			              "' src='../images/ajax-loader.gif' style='position:absolute;top:" +
			              $(this).offset().top + "px;left:" + $(this).offset().left + "px' />");
			
			var dataSource = users_GetAll(CURRENT_USER.Id);
			var minChars = 0;
			if (dataSource.length > MAX_AUTOCOMPLETE_ITEMS){
				minChars = 2;	
			}

			$( this ).autocomplete({
				minLength: minChars,
				source: dataSource,
				focus: function( event, ui ) {
					$( this ).val( ui.item.value );
					return false;
				},
				select: function( event, ui ) {
					$( this ).val( ui.item.value );
					$( this.id + "-id" ).val( ui.item.key );
					USER_ID_RUN_AS = ui.item.key;
					//onUserSelected(ui.item.key);
					return false;
				}
			}).data("autocomplete")._renderItem = function(ul,item) {

				return $( "<li class='ui-widget'></li>" )
					.data( "item.autocomplete", item )
					.append( "<a>" + item.value + "</a>" )
					.appendTo( ul );
			};
			
			this.autoCompleteRendered = true;

			//remove the load image?
			$("#img" + imgRandomID).remove();

		}

		$( this ).autocomplete("search");

	});	

}

function associativeArrayLength(array) {
	var itemCount = 0;
	for (value in array) {
		itemCount += 1;
	}
    return itemCount;
}

function authorizePageViewAdmin(){
	var userId = USER_ID_ACTUAL;
	if (IS_RUN_AS){ userId = USER_ID_RUN_AS;}
	
	if (!userIsAdmin(userId)) {
		var newUrl = URL_CLIENT_UI + '/noAccess.html';
		window.location = newUrl;
	}	
}

function authorizePageView(viewId){
	var userId = USER_ID_ACTUAL;
	if (IS_RUN_AS){ userId = USER_ID_RUN_AS;}
	
	if (!user_HasAccess(userId, viewId)) {
		var newUrl = URL_CLIENT_UI + '/noAccess.html';
		window.location = newUrl;
	}	
}

function bookMarksContainsAssetId(assetId){
	
	var BOOKMARKS = $.cookies.get( COOKIE_BOOKMARKS );
	if (BOOKMARKS === null){
		if ((IS_RUN_AS) && (USER_ID_RUN_AS !== null)){
			BOOKMARKS = bookMarks_Get(USER_ID_RUN_AS);			
		}
		else {
			BOOKMARKS = bookMarks_Get(USER_ID_ACTUAL);			
		}
		$.cookies.set( COOKIE_BOOKMARKS, BOOKMARKS );
	}
	
	if (BOOKMARKS === null) { return false; }
	
	var isBookmarked = false;
	$.each(BOOKMARKS, function(index, item) {
		if (item.AssetId.toLowerCase() == assetId.toLowerCase()) {
			isBookmarked = true;
			return false;
		}
	});
	return isBookmarked;
}

function bookMarksContainReportId(reportId){

	if (BOOKMARKS === null) {
		if ((IS_RUN_AS) && (USER_ID_RUN_AS !== null)){
			BOOKMARKS = bookMarks_Get(USER_ID_RUN_AS);			
		}
		else {
			BOOKMARKS = bookMarks_Get(USER_ID_ACTUAL);			
		}
	}
	
	if (BOOKMARKS === null) { return false; }
	
	var isBookmarked = false;
	$.each(BOOKMARKS, function(index, item) {
		if (item.AssetType == 'Reports'){
			if (item.ReportId.toLowerCase() == reportId.toLowerCase()) {
				isBookmarked = true;
				return false;
			}
		}

	});
	return isBookmarked;	
}

function initMenu(){

	$("ul.subnav").parent().append("<span></span>"); //Only shows drop down trigger when js is enabled (Adds empty span tag after ul.subnav*)
	
	$("ul.topnav li span").click(function() { //When trigger is clicked...

		//Following events are applied to the subnav itself (moving subnav up and down)
		$(this).parent().find("ul.subnav").slideDown('fast').show(); //Drop down the subnav on click

		$(this).parent().hover(function() {
		}, function(){
			$(this).parent().find("ul.subnav").slideUp('slow'); //When the mouse hovers out of the subnav, move it back up
		});

		//Following events are applied to the trigger (Hover events for the trigger)
		}).hover(function() {
			$(this).addClass("subhover"); //On hover over, add class "subhover"
		}, function(){	//On Hover Out
			$(this).removeClass("subhover"); //On hover out, remove class "subhover"
			}			
		);
}

function buildHeaderContent_admin(){

	var homePage = URL_CLIENT_UI + '/default.html';

	var html = [];
	html.push('<table><tr><td><img src="../images/bglam-sm.png" style="float:left;margin-left:10px;" onclick="window.location = \'' + homePage + '\'"/></td>');
	html.push('<td><h1 class="header">Bechtel GLobal Asset Management (BGLAM)</h1></td>');
	if (CURRENT_NODE != 'PROD'){
		var instanceName = null;
		switch (CURRENT_NODE){
			case 'LOCAL': 
				instanceName = 'Local';
				break;
			case 'DEV':
				instanceName = 'Development';
				break;
			case 'QA':
				instanceName = 'QA';
				break;
			case 'STG':
				instanceName = 'Staging';
				break;				
		}
		html.push('<td><h1 class="header"> - ' + instanceName + '</h1></td>');
	}
	html.push('</tr></table>');
	return html.join('');
}

function buildHeaderContent(){
	
	var homePage = URL_CLIENT_UI + '/default.html';

	var html = [];
	html.push('<table><tr><td><img src="images/bglam-sm.png" style="float:left;margin-left:10px;" onclick="window.location = \'' + homePage + '\'"/></td>');
	html.push('<td><h1 class="header">Bechtel GLobal Asset Management (BGLAM)</h1></td>');
	if (CURRENT_NODE != 'PROD'){
		var instanceName = null;
		switch (CURRENT_NODE){
			case 'LOCAL': 
				instanceName = 'Local';
				break;
			case 'DEV':
				instanceName = 'Development';
				break;
			case 'QA':
				instanceName = 'QA';
				break;
			case 'STG':
				instanceName = 'Staging';
				break;				
		}
		html.push('<td><h1 class="header"> - ' + instanceName + '</h1></td>');
	}
	html.push('</tr></table>');
	return html.join('');
}

function buildMenuHtml(){

	getCurrentUser();

	var userId = null;
	var isAdmin = false;

	if ((IS_RUN_AS) && (USER_ID_RUN_AS !== null)){
		userId = USER_ID_RUN_AS;
		isAdmin = userIsAdmin(userId);
	}
	else {
		userId = USER_ID_ACTUAL;
		isAdmin = CURRENT_USER_IS_ADMIN;
	}

	var html = [];
	html.push('<ul class="topnav">');
	html.push('<li><a href="' + URL_CLIENT_UI + '/default.html">BGLAM Home</a></li>');
	html.push('<li>');
	html.push('<a>Asset Editor</a>');
	html.push('<ul class="subnav">');
	
	var views = $.cookies.get( COOKIE_VIEWS );
	if (views === null){
		views = views_GetForUser(userId);
		$.cookies.set( COOKIE_VIEWS, views );	
	}	
	 
	if (views !== null){
		var count = views.length;
		for (var i=0; i<count; i++){
			var view = views[i];
			var qs = '?viewId=' + view['key'];		
			html.push('<li><a href="' + URL_CLIENT_UI + '/view.html' + qs + '">' + view['value'] + '</a></li>');
		}
	}
	
	html.push('</ul>');
	html.push('</li>');
	//html.push('<li><a href="' + URL_CLIENT_UI + '/reporting/predefined.html">Reporting</a></li>');	

	html.push('<li>');
	html.push('<a>Reporting</a>');
	html.push('<ul class="subnav">');
	html.push('<li><a href="' + URL_CLIENT_UI + '/reporting/predefined.html">Reports</a></li>');
	html.push('<li><a href="' + URL_CLIENT_UI + '/reporting/myInfo.html">My Info</a></li>');
	if (isAdmin){
		html.push('<li><a href="' + URL_CLIENT_UI + '/reporting/reportBuilder.html">Builder</a></li>');
	}
	html.push('</ul>');
	html.push('</li>');
	
	if (SHOW_BETA_MENUS){

// BEGIN TEST
		var htmlTemp = [];		
		htmlTemp.push('<li>');
		htmlTemp.push('<a href="#">Asset Editor</a>');
		htmlTemp.push('<ul class="subnav">');
		
		htmlTemp.push('<li>');
		htmlTemp.push('<a href="#">Contracts</a>');
		htmlTemp.push('<ul class="subnav">');
		htmlTemp.push('<li><a href="' + URL_CLIENT_UI + '/reporting/chartBuilder.html">Builder</a></li>');
		htmlTemp.push('<li><a href="' + URL_CLIENT_UI + '/reporting/systemMetrics.html">System Metrics</a></li>');		
		htmlTemp.push('</ul>');
		htmlTemp.push('</li>');
		
		htmlTemp.push('<li>');
		htmlTemp.push('<a href="#">Software</a>');
		htmlTemp.push('<ul class="subnav">');
		htmlTemp.push('<li><a href="' + URL_CLIENT_UI + '/reporting/chartBuilder.html">Builder</a></li>');
		htmlTemp.push('<li><a href="' + URL_CLIENT_UI + '/reporting/systemMetrics.html">System Metrics</a></li>');		
		htmlTemp.push('</ul>');
		htmlTemp.push('</li>');		
		
		htmlTemp.push('</ul>');
		htmlTemp.push('</li>');
		//html.push(htmlTemp.join(''));
// END TEST
	
		html.push('<li>');
		html.push('<a href="#">Charts</a>');
		html.push('<ul class="subnav">');
		html.push('<li><a href="' + URL_CLIENT_UI + '/reporting/charts.html">Dashboard</a></li>');
		if (isAdmin){
			html.push('<li><a href="' + URL_CLIENT_UI + '/reporting/chartBuilder.html">Builder</a></li>');
			html.push('<li><a href="' + URL_CLIENT_UI + '/reporting/systemMetrics.html">System Metrics</a></li>');
		}
		html.push('</ul>');
		html.push('</li>');
	
	}
	
	html.push('<li><a href="' + URL_CLIENT_UI + '/bookmarks.html">Favorites</a></li>');
	
	if (SHOW_BETA_MENUS){
		html.push('<li>');
		html.push('<a href="#">Settings</a>');
		html.push('<ul class="subnav">');
		html.push('<li><a href="' + URL_CLIENT_UI + '/settings/alerts.html">Alerts & Notifications</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/settings/preferences.html">Preferences</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/settings/activeRoles.html">Roles & Permissions</a></li>');
		html.push('</ul>');
		html.push('</li>');
	}

	var showUtils = false;
	
	if (IS_RUN_AS){
		showUtils = USER_RUN_AS.AllowBulkUploads;
	}
	else {
		showUtils = CURRENT_USER.AllowBulkUploads;
	}

	if (showUtils){
		html.push('<li>');
		html.push('<a href="#">Utilities</a>');
		html.push('<ul class="subnav">');
		//html.push('<li><a href="' + URL_CLIENT_UI + '/utils/templateGenerator.html">Bulk Upload Templates</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/utils/uploader.html">Bulk Uploader</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/utils/uploaderLogs.html">Bulk Upload Logs</a></li>');
		if (isAdmin){ html.push('<li><a href="' + URL_CLIENT_UI + '/utils/images.html">System Images</a></li>'); }
		html.push('</ul>');
		html.push('</li>');			
	}	

	if (isAdmin){
		html.push('<li>');
		html.push('<a>Admin</a>');
		html.push('<ul class="subnav">');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/assetTypes.html">Asset Types</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/helpEditor.html">Help Editor</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/picklists.html">Picklists</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/properties.html">Properties</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/propertyGroups.html">Property Groups</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/views.html">Views</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/roles.html">Roles</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/serviceModules.html">Service Modules</a></li>');
		html.push('<li><a href="' + URL_CLIENT_UI + '/admin/processingService.html">Processing Service</a></li>');
		html.push('</ul>');
		html.push('</li>');
	}

	html.push('<li><a href="' + URL_CLIENT_UI + '/help.html">Help</a></li>');
	html.push('</ul>');
	return html.join('');
}

//On Hover Over
function megaHoverOver(){
    $(this).find(".sub").stop().fadeTo('fast', 1).show(); //Find sub and fade it in
    (function($) {
        //Function to calculate total width of all ul's
        jQuery.fn.calcSubWidth = function() {
            rowWidth = 0;
            //Calculate row
            $(this).find("ul").each(function() { //for each ul...
                rowWidth += $(this).width(); //Add each ul's width together
            });
        };
    })(jQuery); 

    if ( $(this).find(".row").length > 0 ) { //If row exists...

        var biggestRow = 0;	

        $(this).find(".row").each(function() {	//for each row...
            $(this).calcSubWidth(); //Call function to calculate width of all ul's
            //Find biggest row
            if(rowWidth > biggestRow) {
                biggestRow = rowWidth;
            }
        });

        $(this).find(".sub").css({'width' :biggestRow}); //Set width
        $(this).find(".row:last").css({'margin':'0'});  //Kill last row's margin

    } else { //If row does not exist...

        $(this).calcSubWidth();  //Call function to calculate width of all ul's
        $(this).find(".sub").css({'width' : rowWidth}); //Set Width

    }
}
//On Hover Out
function megaHoverOut(){
  $(this).find(".sub").stop().fadeTo('fast', 0, function() { //Fade to 0 opactiy
      $(this).hide();  //after fading, hide it
  });
}

// querystring ops

function getQueryStrings(){
	QS_VIEW_ID = getParameterByName('viewId');
	QS_ASSET_ID = getParameterByName('assetId');
	QS_ASSETTYPE_ID = getParameterByName('assetTypeId');
	NEW_ITEM_ID = getParameterByName('newAssetId');
	VIEW_FROM_PROCESS = getParameterByName('fromProcess');
	//QS_MODE = getParameterByName('mode');
	//if (QS_MODE == undefined) { QS_MODE = MODE_EDIT; }
}

function clearQueryStrings(){
	NEW_ITEM_ID = null;
	QS_ASSET_ID = null;
	QS_ASSETTYPE_ID = null;
	QS_VIEW_ID = null;
	VIEW_FROM_PROCESS = false;
	//QS_MODE = MODE_VIEW;
}

// end querystring ops

// Filter Dialog Methods

function createFilterDialog(assetTypeId, isInstance, assetNameLabel){
	
	ASSET_FILTERS = [];
	FILTERED_ASSET_TYPE_ID = assetTypeId;
	FILTERED_ASSET_IS_INSTANCE = isInstance;
	
	var html = [];
	html.push('<table style="width:100%;">');
	html.push('<tr><td colspan=2>Attributes</td>');
	html.push('<td colspan=2>Attribute Values</td></tr>');
	
	// load available attributes for the provided assetTypeId
	var optionValues = '<option value="-1">[Select]</option>';
	
	var assetRequestType = null;
	if (isInstance){
		assetRequestType = ART_INSTANCE;
	}
	else {
		assetRequestType = ART_DEFINITION;		
	}

	$.each(properties_Get(assetTypeId, assetRequestType, true, FILTERED_ASSET_IS_INSTANCE, true), function(index, value){
		optionValues += '<option value="' + value['Key'].toLowerCase() + '">' + value['Value'] + '</option>';
	});
	
	html.push('<tr><td style="width:45%;"><select id="properties" style="width:100%;" class="ui-widget ui-state-default" onChange="setTimeout(function() { selectedAttributeChanged(); }, 100)">');
	html.push(optionValues);
	html.push('</select></td>');
	html.push('<td style="width:5%;text-align:center;">=</td>');
	html.push('<td style="width:45%;" id="tdFilterPropVals"><select id="propertyValues" disabled="disabled" style="width:100%;" onChange="setTimeout(function() { selectedAttributeValueChanged(); }, 100)" class="ui-widget ui-state-default"></select></td>');
	html.push('<td style="width:5%;"><input id="btnAdd" type="button" value="Add Filter" onClick="addFilter()" disabled="disabled"></input></td></tr>');
	html.push('<tr><td colspan=4 style="width:100%;"><div id="currentFilters"></div></td></tr>');
	html.push('<tr><td colspan=4 style="width:100%;">');

	assetRequestType = null;
	
	var assetCount = 0;
	if (isInstance){
		assetRequestType = ART_INSTANCE;
		assetCount = assets_GetInstanceCount(assetTypeId, includeChildAssetTypes);
	}
	else {
		assetRequestType = ART_DEFINITION;
		assetCount = assets_GetCount(assetTypeId);
	}
	
	if (assetCount < MAX_AUTOCOMPLETE_ITEMS){

		html.push('<select id="filteredAssetIds" size=10 style="width:100%" class="ui-widget">');

		var assets = assets_Get(assetTypeId, assetRequestType, includeChildAssetTypes);

		var options = [];
		var length = assets.length;
		for (var i=0; i<length; i++){
			options[i] = '<option value="' + assets[i]['Key'] + '">' + assets[i]['Value'] + '</option>';
		}
		html.push(options.join(''));
		html.push('</select>');
	}
	else {
		html.push('<select id="filteredAssetIds" size=10 style="width:100%;display:none" class="ui-widget"></select>');
	}
	
	var assetTypeName = assetType_GetName(FILTERED_ASSET_TYPE_ID, (assetCount > 1));	

	html.push('</td></tr>');
	html.push('<tr><td><div id="divSpinner" style="overflow-y:auto;height:100%;width:100%;display:none;"></div></td></tr>');
	html.push('<tr><td id="tdStatus" colspan=4 style="width:100%;"><i>'+ assetCount + ' ' + assetTypeName.toLowerCase() + ' in the database</i></td></tr>');
	html.push('</table>');

	return html.join('');	

}

function selectedAttributeChanged(){
	
	var selectedValue = $('#properties').val();

	if (selectedValue != '-1'){
		
		if (filterAttributeRequiresTextInput(selectedValue)){

			// change the input for the property value to a text input
			$('#tdFilterPropVals').html(propertyValues_inputHtml);
			
			// enable the input
			$('#propertyValues').removeAttr('disabled');
			
			$('#btnAdd').removeAttr('disabled');	
			$('#btnAdd').val('Search');
			
		}
		else {
			
			// change the input for the property value to a select
			$('#tdFilterPropVals').html("Please wait ...");
			
			// disable the input
			$('#propertyValues').attr('disabled', 'disabled');
						
			$('#btnAdd').val('Add Filter');
			
			//var selectedPropertyId = sender.value;
			var selectedPropertyId = selectedValue;

			var potentialValues = null;
			
			var assetIds = filteredAssetIds();
			
			if ((assetIds !== null) && (associativeArrayLength(assetIds) > 0)) {
				potentialValues = propertyValues_GetPotential(assetIds, selectedPropertyId);
			}
			else {
				
				var searchRequestType = ART_DEFINITION; // default to Definitions				
				if (FILTERED_ASSET_IS_INSTANCE) { searchRequestType = ART_INSTANCE; }
				
				potentialValues = propertyValues_GetPotentialByAssetType(FILTERED_ASSET_TYPE_ID, selectedPropertyId, searchRequestType);
			}		
			
			// clear any previously-displayed data
			selectOptionsRemove('propertyValues');
			selectOptionAppend('propertyValues', -1, '[Select]', true);
			
			var html = [];
			html.push(propertyValues_selectHtml);
			html.push('<option value="-1">[Select]</option>');
			if (potentialValues !== null){
				// display all of the potential property values
				var length = potentialValues.length;
				for (var i=0; i<length; i++){
					var displayValue = potentialValues[i]['Value'];
					html.push('<option value="' + potentialValues[i]['Key'].toLowerCase() + '" title="' + displayValue + '">' + displayValue + '</option>');
				}
			}
			html.push('</select>');
			$('#tdFilterPropVals').html(html.join(''));
			
			// enable the input
			$('#propertyValues').removeAttr('disabled');
		}
	}
	else {
		// disable the property value input
		$('#propertyValues').attr('disabled', 'disabled');
	}	
}

function selectedAttributeValueChanged(sender){
	
	var selectedValue = $('#propertyValues').val();

	if (selectedValue != -1)	{
		$('#btnAdd').removeAttr('disabled');
	}
	else {
		$('#btnAdd').attr('disabled', 'disabled');	
	}
}

function filteredAssetIds(){
	var ids = [];
	$('#filteredAssetIds option').each(function(index,value){	
		ids.push(value.value);	
	});
	return ids;	
}

function addFilter(){

	// disable the add filter button
	$('#btnAdd').attr('disabled', 'disabled');

	$('#filteredAssetIds').hide();
	$("#divSpinner").html("<img src='images/ajax-loader.gif'/>");
	$('#divSpinner').show();

	// get the id of the property we're going to filter/search on
	var propertyId = $('#properties').val();

	// get the name of the property we're going to filter/search on
	var propertyName = selectOption_GetCurrentText('properties');
		
	// get the value to match on
	var valueToMatch = $('#propertyValues').val();
	
	// if the user didn't enter a value to search for or select a value to match on, just exit
	if ((valueToMatch === null) || (valueToMatch == '')) { return; }

	var propertyValueDisplay;	 
	
	// create a variable to hold the matched results [AssetID]:[AssetName]
	var matchedAssetIds = {};

	// if the selected property is the Asset's name
	if (propertyId == PROP_ASSET_NAME){
		matchedAssetIds = getMatchesViaSearch(valueToMatch);
		propertyValueDisplay = valueToMatch;		
	}
	else {
		propertyValueDisplay = selectOption_GetCurrentText('propertyValues');
		//alert('propertyId: ' + propertyId + ' valueToMatch: ' + valueToMatch);
		matchedAssetIds = getMatchesViaPropertyValue(propertyId, valueToMatch);
	}

	// clear any previous results
	selectOptionsRemove('filteredAssetIds');
	
	// add the matched list of asset ids returned from the server
	if ((matchedAssetIds !== null) && (associativeArrayLength(matchedAssetIds) > 0)){
		if (propertyId == 'AssetName') {
			for (value in matchedAssetIds) {
				var k = matchedAssetIds[value]['Key'];
				var v = matchedAssetIds[value]['Value'];
				selectOptionAppend('filteredAssetIds', k, v, false);
			}
		}
		else {
			var html = [];
			var length = matchedAssetIds.length;
			for (var i=0; i<length; i++){
				//html[i] = '<option value="' + matchedAssetIds[i]['Key'] + '">' + matchedAssetIds[i]['Value'] + '</option>';
				html.push('<option value="' + matchedAssetIds[i]['Key'] + '">' + matchedAssetIds[i]['Value'] + '</option>');
			}
			$('#filteredAssetIds').html(html.join(''));
		}		
	}
	
	// cache the filter
	ASSET_FILTERS[propertyId] = valueToMatch;
	
	// remove the filter property from the list of available properties
	htmlSelect_RemoveOptionByValue('properties', propertyId);

	// reset the property selector back to [Select]
	$('#properties').val('-1');	

	if (propertyId == PROP_ASSET_NAME) {
		// set the text input to an empty string
		$('#propertyValues').val('');		
	}
	else {
		// remove the property values from the select list
		selectOptionsRemove('propertyValues');
		selectOptionAppend('propertyValues', -1, '[Select]', false);
		
		// set the property value selector back to [Select]
		$('#propertyValues').val('-1');		
	}
	
	// disable the property value input
	$('#propertyValues').attr('disabled', 'disabled');
	
	// display the number of matched asset for the user (mainly for some visual feedback that something happened)
	if ((matchedAssetIds !== null) && (associativeArrayLength(matchedAssetIds) > 0)){
		if (propertyId == 'AssetName') {
			updateNumberOfMatches(FILTERED_ASSET_TYPE_ID, associativeArrayLength(matchedAssetIds));
		}
		else {
			updateNumberOfMatches(FILTERED_ASSET_TYPE_ID, matchedAssetIds.length);	
		}		
	}
	else {
		updateNumberOfMatches(FILTERED_ASSET_TYPE_ID, 0);
	}
	
	// update the list of filters for the user
	addFilterToUserDisplay(propertyId, propertyName, propertyValueDisplay);
	
	// save the filter
	var filter = new Object();
		filter.propertyId = propertyId;
		filter.propertyName = propertyName;
		filter.propertyValue = valueToMatch;
		filter.propertyValueDisplay = propertyValueDisplay;
		FILTERS[FILTERS.length] = filter;
	
	// ensure the match list is visible
	$('#filteredAssetIds').show();
	$('#divSpinner').hide();
}

function getMatchesViaSearch(valueToSearch){
	
	valueToSearch = valueToSearch.toLowerCase();
	
	var assetIds = htmlSelect_GetAllValues('filteredAssetIds');
	
	var length = associativeArrayLength(assetIds);
	
	if ((assetIds === null) || (length == 0)) {
		
		var searchRequestType = ART_DEFINITION; // default to Definitions
		
		if (FILTERED_ASSET_IS_INSTANCE) { searchRequestType = ART_INSTANCE; }
		
		return assets_SearchByName(FILTERED_ASSET_TYPE_ID, searchRequestType, valueToSearch, true);
		
	}
	else {
		
		//var matchedAssetIds = {};
		var matchedAssetIds = [];
		
		//var regExp = new RegExp('\\b'+valueToSearch+'\\w+\\b', 'gi');
		//var regExp = new RegExp(/valueToSearch/\\w, 'gi');

		var matchCount = 0;
		for (key in assetIds){

			var assetName = assetIds[key];

			//var matchPos = assetName.match(/valueToSearch/i);
			var matchPos = assetName.toLowerCase().indexOf(valueToSearch);
			
			if (matchPos > -1) {
				var match = new Object();
				match.Key = key;
				match.Value = assetName;
				matchedAssetIds[matchCount] = match;
				matchCount++;
			}			
		}
		
		return matchedAssetIds;
	}	
}

function getMatchesViaPropertyValue(propertyId, propertyValue){
	
	// do we already have a subset of asset ids to work with?
	var assetIds = filteredAssetIds();	
	
	if ((assetIds === null) || (associativeArrayLength(assetIds) == 0)) {
		// no, we don't.  perform the match based on the asset type

		var searchRequestType = ART_DEFINITION; // default to Definitions
		
		if (FILTERED_ASSET_IS_INSTANCE) { searchRequestType = ART_INSTANCE; }		
			
		return assets_GetMatchingByAssetTypeId(FILTERED_ASSET_TYPE_ID, searchRequestType, propertyId, propertyValue, true);
	}
	else {
		// yes, we do.  we'll use just these asset ids for our filtering
		// get the asset ids matching the filter(s)
		return assets_GetMatching(assetIds, propertyId, propertyValue, true);		
	}	

}

function refreshFilters(){
	
	// clear any previous results
	selectOptionsRemove('filteredAssetIds');
		
	if ((ASSET_FILTERS !== null) && (associativeArrayLength(ASSET_FILTERS) > 0)) {
		var matchedAssetIds = assets_GetByPropertyValues(FILTERED_ASSET_TYPE_ID, ASSET_FILTERS);
		
		// clear any previous results
		selectOptionsRemove('filteredAssetIds');
		
		// add the matched list of asset ids returned from the server
		$.each(matchedAssetIds, function(index, value){
			selectOptionAppend('filteredAssetIds', value['Key'].toLowerCase(), value['Value'], false);
		});		
	}
	else {
		if (FILTERED_ASSET_IS_INSTANCE){
			$.each(assets_GetInstances(FILTERED_ASSET_TYPE_ID, includeChildAssetTypes), function(index, value){
				selectOptionAppend('filteredAssetIds', value['Key'].toLowerCase(), value['Value'], false);
			});
		}
		else {
			$.each(assets_GetDictionary(FILTERED_ASSET_TYPE_ID, false), function(index, value){
				selectOptionAppend('filteredAssetIds', value['Key'].toLowerCase(), value['Value'], false);
			});		
		}
	}
	
	// display the number of matched asset for the user (mainly for some visual feedback that something happened)
	var itemCount = $('#filteredAssetIds option').size();
	$('#tdStatus').html('<i>Found ' + itemCount + ' assets</i>');
	
}

function removeFilter(sender){

	$('#filteredAssetIds').hide();
	$("#divSpinner").html("<img src='images/ajax-loader.gif'/>");
	$('#divSpinner').show();
	
	// delete the cached filter info
	delete ASSET_FILTERS[sender.id];
	
	// remove the filter from the UI
	var controlId = sender.id + '_parent';
	$('#' + sender.id).remove();	
	$('#' + controlId).prev().remove();
	$('#' + controlId).remove();
	
	// add the filter property back to the list of available properties
	// no easy way to get the property's display value from the UI; just grab it from the database
	var propertyName = property_GetName(sender.id);
	htmlSelect_InsertOption('properties', sender.id, propertyName, false);
	
	refreshFilters();
	
	$('#filteredAssetIds').show();
	$('#divSpinner').hide();
}

function addFilterToUserDisplay(propertyId, propertyName, propertyValueDisplay){
	// display the filter(s) to the user
	var currentHtml = $('#currentFilters').html();
	
	if ((currentHtml !== null) && (currentHtml !== '')) {
		$('#currentFilters').html(currentHtml + '<span id="' + propertyId + '_parent">' + propertyName + ' = ' + propertyValueDisplay + '<span id="' + propertyId + '" class="ui-icon ui-icon-circle-close" style="float: right; margin-right: .2em;" onClick="removeFilter(this)"/></span><br/>');		
	}
	else {
		$('#currentFilters').html('<span id="' + propertyId + '_parent">' + propertyName + ' = ' + propertyValueDisplay + '<span id="' + propertyId + '" class="ui-icon ui-icon-circle-close" style="float: right; margin-right: .2em;" onClick="removeFilter(this)"/></span><br/>');	
	}	
}

function updateNumberOfMatches(assetTypeId, numberOfMatches){
	var assetTypeName = assetType_GetName(assetTypeId, (numberOfMatches > 1));
	$('#tdStatus').html('<i>Found ' + numberOfMatches + ' ' + assetTypeName.toLowerCase() + '</i>');
}

function filterAttributeRequiresTextInput(propertyId){
	var property = property_Get(propertyId);
	if (property.DataType == SYSTEM_TYPE_ASSET_NAME) { return true; }
	if (property.DataType == DATA_TYPE_STRING) { return true; }
	if (property.DataType == DATA_TYPE_MEMO) { return true; }
	if (property.DataType == DATA_TYPE_SYSTEM_DESCRIPTION) { return true; }
	return false;
}

// End Filter Dialog methods

// Add User code
function getEmployeeData(val)
{
	//SVC_ADDRESSBOOK
//ROLLING THIS BACK TO REV286 CODE
/*	  ajaxCallPlus(SVC_ADDRESSBOOK_GETEMPLOYEEDATABYBUN, 
	             '{"userId":"amason"}', 
				 function()
				 {
					alert('success');
				 }, 
				 function(xhr, ajaxOptions, thrownError)
				 {
					alert('error');
				 }
				 );
*/
	var retVal = null;

	$.ajax({
		type: "GET",				
	  	url: SVC_USERDATA_GET + '?userId=' + val,
  	  	dataType: "json",
		contentType: "json",
		async: false,
	 	error: function(XMLHttpRequest, textStatus, errorThrown) {
	  		alert(textStatus + ' ' + errorThrown);
  	  	},
	  	success: function(data) {
	  		/*if (data)
	  		{
	  			alert(data.DisplayName);			
	  		} else
	  		{
	  			alert('');
	  		}*/
	  		retVal = data;
	  }
	});
	
	return retVal;				 
			
}

function isUserInBGLAM(val)
{
	var retVal = null;

	$.ajax({
		type: "GET",				
	  	url: SVC_USERID_GET + '?userId=' + val,
  	  	dataType: "json",
		contentType: "json",
		async: false,
	 	error: function(XMLHttpRequest, textStatus, errorThrown) {
	  		alert(textStatus + ' ' + errorThrown);
  	  	},
	  	success: function(data) {
			retVal = data;
	  	}
	});
	
	return retVal;
}

function createUserInBLGAM(id, userName, lName, fName, mName)
{

	var retVal = null;
	var url = SVC_USER_CREATE;
	
	var jsonData;
	jsonData = '{"id":"' + id + '",';
	jsonData += '"userName":"' + userName + '",';
	jsonData += '"lName":"' + lName + '",';
	jsonData += '"fName":"' + fName + '",';
	jsonData += '"mName":"' + mName + '"}';
			
		
	
	$.ajax({
		type: "POST",				
	  	url: url ,
  	  	dataType: "json",
		contentType: "application/json; charset=utf-8",
                data: jsonData,
		async: false,
	 	error: function(XMLHttpRequest, textStatus, errorThrown) {
	  		//alert(textStatus + ' ' + errorThrown);
  	  	},
	  	success: function(data) {
	  	//alert('created');
	  	
	  }
	});
	
	return retVal;
}

function getGUIDFromServer(dataType)
{
	var retVal = null;

	$.ajax({
		type: "GET",				
		url: SVC_GENERATE_GUID + '?dataType=' + dataType,
  	  	dataType: "json",
		contentType: "json",
		async: false,
	 	error: function(XMLHttpRequest, textStatus, errorThrown) {
	  		alert(textStatus + ' ' + errorThrown);
  	  	},
	  	success: function(data) {
	  	
	  		if (data)
	  		{
	  			retVal = data;			
	  		} else
	  		{
	  			retVal = "";			
	  		}
	  }
	});
	
	return retVal;
}
// End Add Users

function createUrlForBookmark(anchorId, viewId, assetId, assetName){
	return '<a href="' + URL_CLIENT_UI + '/' + PAGE_NAME_VIEW + '?viewId=' + viewId + '&assetId=' + assetId + '" target="_blank">' + assetName + '</a>';
}

function createUrlForBookmarkedReport(anchorId, reportId, reportName){
	return '<a href="' + URL_CLIENT_UI + '/reporting/' + PAGE_NAME_REPORTS + '?reportId=' + reportId + '" target="_blank">' + reportName + '</a>';	
}

function createUrlForView(anchorId, viewId, assetId, assetName){
	var html = '<a id="' + anchorId + '" href="javascript:parent.renderView(\'';
	html += viewId + '\', \'' + assetId + '\')"';
	html += ' style="display:block;float:left;" >';
	html += assetName + '</a>';
	return html;	
}

function createViewUrl(anchorId, viewId, assetId, assetName){
	
	var html = [];

	html.push("<a id=\"" + anchorId + "\" href=\"" + URL_CLIENT_UI);
	html.push("/" + PAGE_NAME_VIEW + "?viewId=");
	html.push(viewId + "&assetId=" + assetId);
	html.push("\" style=\"display:block; float:left;\" >");
	html.push(assetName + "</a>");

	return html.join('');
}

function createViewUrlPlain(viewId, assetId, assetTypeId, newAssetId, fromProcess){
	
	var html = URL_CLIENT_UI + '/' + PAGE_NAME_VIEW + '?viewId=';
		html += viewId + '&assetId=' + assetId;
	
		if (assetTypeId == undefined) { assetTypeId = null; }
		html += '&assetTypeId=' + assetTypeId;
		
		if (newAssetId == undefined) { newAssetId = null; }
		html += '&newAssetId=' + newAssetId;
		
		if (fromProcess == undefined) { fromProcess = false; }
		html += '&fromProcess=' + fromProcess;					

	//alert(html);

	return html;
}

function formatBackslashes(value){
	var temp = value.split('\\');
	var returnValues = [];
	var l = temp.length;
	for (var i=0; i < l; i++){
		if (i == (l-1)){
			returnValues[i] = temp[i];
		}
		else {
			//returnValues[i] = temp[i] + '\\\\\\\\';
			returnValues[i] = temp[i] + '&#92;';
		}
	}
	return returnValues.join('');
}

// drives the list of currencies that are shown within the UI
function getCurrencies(){
	var array = new Array();
	
	var obj = new Object();
	obj.value = "$";
	obj.key = "$";
	array[array.length] = obj;

	obj = new Object();
	obj.value = "C$";
	obj.key = "C$";
	array[array.length] = obj;

	obj = new Object();
	obj.value = "€";
	obj.key = "€";
	array[array.length] = obj;

	obj = new Object();
	obj.value = "£";
	obj.key = "£";
	array[array.length] = obj;

	obj = new Object();
	obj.value = "¥";
	obj.key = "¥";
	array[array.length] = obj;

	// Indonesian Rupiahs
	obj = new Object();
	obj.value = "Rp";
	obj.key = "Rp";
	array[array.length] = obj;

	// Indian Rupees
	obj = new Object();
	obj.value = "Rs";
	obj.key = "Rs";
	array[array.length] = obj;

	// Singaporean Dollar
	obj = new Object();
	obj.value = "S$";
	obj.key = "S$";
	array[array.length] = obj;

	// Saudi Arabia Riyal
	obj = new Object();
	obj.value = "ريال (SAR)";
	obj.key = "ريال (SAR)";
	array[array.length] = obj;

	return array;	
}

function getDialogButton( dialog_selector, button_name )
{
  var buttons = $( dialog_selector + ' .ui-dialog-buttonpane button' );
  for ( var i = 0; i < buttons.length; ++i )
  {
	 var jButton = $( buttons[i] );
	 if ( jButton.text() == button_name )
	 {
		 return jButton;
	 }
  }

  return null;
}

function getParameterByName(name)
{
  name = name.replace(/[\[]/,"\\\[").replace(/[\]]/,"\\\]");
  var regexS = "[\\?&]"+name+"=([^&#]*)";
  var regex = new RegExp( regexS );
  var results = regex.exec( window.location.href );
 
  if( results == null ){
  	return "";
  }    
  else {
  	return decodeURIComponent(results[1].replace(/\+/g, " "));
  }
    
}

function ajaxCall(url, inputData, callBack, passParameter)
{
	$.ajax({
		url:url,
		type: "POST",
		contentType: "application/json; charset=utf-8",
		data: inputData,
		dataType: "json",
		async: false,
		success: function(msg)
		{
			if (callBack != null)
			{
				if (passParameter != false)
				{
					callBack(msg);
				} else
				{
					callBack();
				}
			}
		},
		error:function (xhr, ajaxOptions, thrownError){
			//alert(xhr.status);
			//alert(xhr.responseText);
			//alert(xhr.responseXML);
			alert( "ERROR");
		}
	});
}	
	
function ajaxCallPlus(url, inputData, onSuccess, onError)
{
	$.ajax({
		url:url,
		type: "POST",
		contentType: "application/json; charset=utf-8",
		data: inputData,
		dataType: "json",
		async: false,
		success: onSuccess,
		error:onError
	});
}		

function ajaxDelete(url) {

	var returnData = null;
	
	$.ajax({
	  type: "DELETE",
	  contentType: "json",
	  dataType: "json",
	  async: false,
	  url: url,
	  error: function(XMLHttpRequest, textStatus, errorThrown) {
	  	alert(XMLHttpRequest.responseText + ' ' + errorThrown);
  	  },
	  success: function(data) {
		returnData = data;	
	  }	  
	});	
	
	return returnData;

}

function ajaxGet(url){

	var returnData = null;
	
	$.ajax({
	  type: "GET",
	  contentType: "json",
	  dataType: "json",
	  async: false,
	  url: url,
	  error: function(XMLHttpRequest, textStatus, errorThrown) {
	  	alert(XMLHttpRequest.responseText + ' ' + errorThrown);
  	  },
	  success: function(data) {
		returnData = data;	
	  }	  
	});	
	
	return returnData;
	
}

function ajaxPost(url, jsonData){

	var returnData = null;
	
	$.ajax({
	    url: url,
	    type: "POST",
	    contentType: "application/json; charset=utf-8",
	    data: jsonData,
	    dataType: "json",
	    async: false,
	  	error: function(XMLHttpRequest, textStatus, errorThrown) {
	  		alert(XMLHttpRequest.responseText + ' ' + errorThrown);
	  	  },
	    success: function(data) {
			returnData = data;	    	
    	}
    });
    
    return returnData;

}

function selectOptionAppend(controlId, value, text, selectNewItem) {
	
	if (selectNewItem) {
		$('#' + controlId).
		  append($("<option></option>").
		  attr("value",value).
		  text(text)).
		  val(value);			
	}
	else {
		$('#' + controlId).
		  append($("<option></option>").
		  attr("value",value).
		  text(text));
	}
}

function autoComplete_HasData(controlId){
	var source = $('#' + controlId).autocomplete( "option", "source" );
	// TODO: This will fail if the autocomplete contains only 1 'real' item
	// If the autocomplete hasn't been rendered, it still returns a length of 1
	// Need to fix this method
	if ((source !== undefined) && (source !== null) && (source.length > 1)){
		return true;
	}
	return false;
}

function autoComplete_ContainsText(textToFind, controlId, isCaseSensitive){
	
	var found = false;
	
	if (!isCaseSensitive) {
		textToFind = textToFind.toLowerCase();
	}
	
	var source = $('#' + controlId).autocomplete( "option", "source" );

	if (!isCaseSensitive){
		for (var i=0; i<source.length; i++){	
			if (source[i]['value'].toLowerCase() == textToFind){
				found = true;
				break;
			}
		}		
	}
	else {
		for (var i=0; i<source.length; i++){
	
			if (source[i]['value'] == textToFind){
				found = true;
				break;
			}
		}		
	}
	
	return found;
	
}

function autoComplete_GetInsertionIndex(newValue, controlId, isCaseSensitive){

	var found = false;
	var index;

	if (!isCaseSensitive) {
		newValue = newValue.toLowerCase();
	}
	
	var source = $('#' + controlId).autocomplete( "option", "source" );

	if (!isCaseSensitive){
		for (var i=0; i<source.length; i++){	
			if (source[i]['value'].toLowerCase() > newValue){
				index = i;
				found = true;
				break;
			}
		}		
	}
	else {
		for (var i=0; i<source.length; i++){	
			if (source[i]['value'] > newValue){
				index = i;
				found = true;
				break;
			}
		}		
	}
	
	if (found) { return index; }
	return -1;
}

function autoComplete_Insert(controlId, newValue, text){

	var insertionIndex = autoComplete_GetInsertionIndex(text, controlId, false);

	if (insertionIndex != -1){
		var source = $('#' + controlId).autocomplete( "option", "source" );
		var thing = new Object();
		thing.key = newValue;
		thing.value = text;
		source.splice(insertionIndex, 0, thing);
		return true;
	}
	return false;
}

function autoComplete_CreateDataSource(newValues){
	var newDs = [];
	var length = associativeArrayLength(newValues);
	
	var i = 0;
	for (assetId in newValues) {
		var thing = new Object();
		thing.key = assetId;
		thing.value = newValues[assetId];
		newDs[i] = thing;
		i++;		
	}
	return newDs;	
}

function autoComplete_RemoveByValue(controlId, keyToRemove){
	
	var newDs = [];
	keyToRemove = keyToRemove.toLowerCase();
	
	var source = $('#' + controlId).autocomplete( "option", "source" );

	var itemCount = 0;
	var length = source.length;
	for (var i=0; i<length; i++){
		var item = source[i];
		if ((item !== undefined) && (item !== null)){
			var key = item['key'] + '';
			if (key.toLowerCase() != keyToRemove){
				var thing = new Object();
				thing.key = key;
				thing.value = item['value'];
				newDs[itemCount] = thing;
				itemCount++;
			}
		}
	}
	
	$('#' + controlId).autocomplete( "option", "source", newDs);
	
	var minChars = 0;
	if (i > MAX_AUTOCOMPLETE_ITEMS){
		minChars = 2;	
	}
	
	$('#' + controlId).autocomplete( "option", "minLength", minChars);	
	
	return true;	
}

function autoComplete_SwapDatasource(newValues, controlId){

	var newDs = autoComplete_CreateDataSource(newValues);

	$('#' + controlId).autocomplete( "option", "source", newDs);
	
	var minChars = 0;
	if (newDs.length > MAX_AUTOCOMPLETE_ITEMS){
		minChars = 2;	
	}
	
	$('#' + controlId).autocomplete( "option", "minLength", minChars);

}

function autoComplete_SwapDatasourceAsIs(newValues, controlId){

	$('#' + controlId).autocomplete( "option", "source", newValues);
	
	var minChars = 0;
	if (newValues.length > MAX_AUTOCOMPLETE_ITEMS){
		minChars = 2;	
	}
	
	$('#' + controlId).autocomplete( "option", "minLength", minChars);
	
}

function checkBox_SetChecked(controlId, isChecked){
	$('#' + controlId).attr('checked', isChecked);	
}

function checkBox_GetChecked(controlId){
	//if ($.get('#'+ controlId).checked){
		//return true;
	//}
	//if ($('#'+ controlId).attr('checked')){
		//return true;
	//}
	//return false;
	return $('#'+ controlId).attr('checked'); 
}

function displayCurrentUser(){

	if (CURRENT_USER_IS_ADMIN){
		var html = '<span class="ui-icon ui-' + ICON_PERSON + '"';
		html += ' title="Run as Another User" onClick="onRunAsSelected()"';	
		html += ' style="float: left; margin-right: .2em;"></span>';
		html += '<label>' + CURRENT_USER.FirstName + ' ' + CURRENT_USER.LastName + '</label>';
		
		if ((IS_RUN_AS) && (USER_ID_RUN_AS !== null)){
			html += '<br><label style="float:right">As: ' + user_GetDisplayName(USER_ID_RUN_AS) + '</label>';
		}

		$('#spanGreeting').html(html);
	}
	else {
		$('#spanGreeting').html('Welcome ' + CURRENT_USER.FirstName);		
	}
	
}

function getCurrentUser() {

	USER_ID_ACTUAL = $.cookies.get( COOKIE_UID );

	if ((USER_ID_ACTUAL == null) || (USER_ID_ACTUAL == undefined)){

		if (IS_DEBUG_MODE)
		{
			USER_ID_ACTUAL = '5ff0b5c6-1dab-47ef-a1d7-86efb05911ab'; // Randall Sexton, Admin
			//USER_ID_ACTUAL = '11387D91-75FF-4924-BC61-4B81C717AAE2'; // Jana Cornell, Contracts
			//USER_ID_ACTUAL = 'B4DC1A4C-71AE-4917-93A7-5CACF6221BEF'; // Steve Bettinger, Software
			//USER_ID_ACTUAL = '52F596E5-71FE-4898-9968-91D9F2062166'; // General Martin, San Fran SAM
			//USER_ID_ACTUAL = '5F469D99-4403-41E6-BD95-83EA618DE9F6'; // Jeff McKay, several SAMs
			//USER_ID_ACTUAL = '9B9E5E07-C21E-483F-A0D2-15DBAEE93389'; // Yvonne Mooers, Frederick SAM
			//USER_ID_ACTUAL = '9812316E-7EC5-4A23-A12B-495F9FF7FFB6'; // Julie Newstead, Contracts
			//USER_ID_ACTUAL = '25C0BFFD-242B-41A3-9982-4A59E4C9B67E'; // Dave Stanislaus, Montreal SAM
			//USER_ID_ACTUAL = '4A8F4BA0-2197-4AD8-B0F6-163369944246'; // Scott Webb, Hardware
			//USER_ID_ACTUAL = 'B4A34D6A-5A74-4328-A037-C9FDA5FBFB20'; // Norm Wollman, Hardware
			//USER_ID_ACTUAL = 'E7328174-7661-4AD6-98CA-27F7C5B61102'; // Subir Sharma, UPM, DPM
		}
		else
		{
			USER_ID_ACTUAL = userId_GetActual();	
		}
		
		$.cookies.set( COOKIE_UID, USER_ID_ACTUAL );

	}
	
	USER_ID_RUN_AS = $.cookies.get( COOKIE_RUN_AS );
	
	if ((USER_ID_RUN_AS !== null) && (USER_ID_RUN_AS != undefined) && (USER_ID_RUN_AS != '')){
		IS_RUN_AS = true;
		USER_RUN_AS = getUser(USER_ID_RUN_AS);
	}
	else {
		IS_RUN_AS = false;
		USER_ID_RUN_AS = null;
		USER_RUN_AS = null;
	}

	CURRENT_USER_ID = USER_ID_ACTUAL;

	CURRENT_USER = $.cookies.get( COOKIE_USER );
	if ((CURRENT_USER == null) || (CURRENT_USER == undefined) || (CURRENT_USER.Id != USER_ID_ACTUAL)) {
		CURRENT_USER = getUser(USER_ID_ACTUAL);
		$.cookies.set( COOKIE_USER, CURRENT_USER );
	}

	CURRENT_USER_IS_ADMIN = $.cookies.get( COOKIE_IS_ADMIN );
	if ((CURRENT_USER_IS_ADMIN == null) || (CURRENT_USER_IS_ADMIN == undefined)){
		CURRENT_USER_IS_ADMIN = userIsAdmin(USER_ID_ACTUAL);
		$.cookies.set( COOKIE_IS_ADMIN, CURRENT_USER_IS_ADMIN );
	}	
	
}

function getIpAddress(){
	var returnData = null;
	
	$.ajax({
	  	type: "GET",
	  	url: "http://jsonip.appspot.com",
	  	dataType: "json",
		contentType: "json",
		async: false,
	  	error: function(XMLHttpRequest, textStatus, errorThrown) {
	  		alert(XMLHttpRequest.responseText + ' ' + errorThrown);
	  	},
	 	success: function(data) {	 		
	 		if ((data !== null) || (data.toString() !== '')) {
	 			returnData = data.ip;				
	 		}			
	  	}
	});
	
	return returnData;	
}

function hideMessage() {
	$("#messageDialog").dialog('close');
	$("#messageDialog").dialog('destroy');
}

function htmlControl_SetEnabled(controlId, isEnabled){
	
	if (isEnabled){
		$('#' + controlId).removeAttr('disabled');	
	}
	else {
		$('#' + controlId).attr('disabled', 'disabled');
	}

}

function htmlControl_SetVisible(controlId, isVisible){
	if (isVisible){
		$('#' + controlId).show();
	}
	else {
		$('#' + controlId).hide();	
	}
}

function htmlEncode(value){
	return $('<div/>').text(value).html();	
}

function htmlSelect_ContainsOptionText(optionText, controlId, isCaseSensitive) {
	
	var found = false;
	
	if (!isCaseSensitive) {
		optionText = optionText.toLowerCase();
	}
	
	$('#' + controlId + ' > option').each(function() {
		
		var tempVal = $(this).val();
		var tempText = $(this).text();
		
		if (!isCaseSensitive) {
			tempText = tempText.toLowerCase();
		}		 
		
		if (optionText == tempText ) { 
			found = true;
			// weird way of breaking out of a loop, i know.
			// blame it on jQuery, not me
			// http://gavinroy.com/jquery-tip-how-to-break-out-of-each
			return false; 			
		}

	});
	
	return found;
}

function htmlSelect_GetAllIds(controlId){
	var values = new Array();
	
	$('#' + controlId + ' > option').each(function() {		
		var tempVal = $(this).val();		
		values.push(tempVal);
	});
	
	return values;	
}

function htmlSelect_GetAllValues(controlId){
	
	var values = {};
	
	$('#' + controlId + ' > option').each(function() {
		
		var tempVal = $(this).val();
		var tempText = $(this).text();
		
		values[tempVal] = tempText;

	});
	
	return values;
}

function htmlSelect_InsertOption(controlId, optionValue, optionText, selectNewItem) {

	var insertionIndex = selectOption_GetInsertionIndex(optionText, controlId);
	
	// TODO: This needs to be worked on to determine if it's an insert before or after
	if (insertionIndex > -1){
		$('#' + controlId + ' option:eq(' + insertionIndex + ')').before('<option value="' + optionValue + '">' + optionText + '</option>');	
	}
	else {
		$('#' + controlId).html('<option value="' + optionValue + '">' + optionText + '</option>');
	}
	
	if (selectNewItem) { $('#' + controlId).val(optionValue); }
	
}

function htmlSelect_RemoveOptionByValue(controlId, value) {
	$('#' + controlId + ' option[value=\'' + value + '\']').remove();	
}

function isArray(obj) {
    return obj.constructor == Array;
}

function isInteger(input){
	return ((isNumeric(input)) && (input.indexOf('.') == -1));
}

function isIPv4(s){

	var temp = s.split('.');

	if (temp.length != 4) { return false; }

	var l = temp.length;
	for (var i=0; i < l; i++){
		if (temp[i].length == 0) { return false; }
	}
	
	return true;
}

function isNumeric(input){
   return (input - 0) == input && input.length > 0;
}

function isUrl(s) {
	var regexp = /(ftp|http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-\/]))?/;
	return regexp.test(s.toLowerCase());
}

function loadAssetTypesAndDefaultViews() {
	
	var defaultViews_Def = $.cookies.get( COOKIE_DEFAULT_VIEWS_DEFINITION );
	var defaultViews_Ins = $.cookies.get( COOKIE_DEFAULT_VIEWS_INSTANCE );

	var userId = USER_ID_ACTUAL;
	if ((IS_RUN_AS) && (USER_ID_RUN_AS !== null)) { userId = USER_ID_RUN_AS; }

	if (defaultViews_Def === null) {
		defaultViews_Def = assetTypes_GetDefaultViews(userId, ART_DEFINITION);
		$.cookies.set( COOKIE_DEFAULT_VIEWS_DEFINITION, defaultViews_Def );
	}
	
	if (defaultViews_Ins === null) {
		defaultViews_Ins = assetTypes_GetDefaultViews(userId, ART_INSTANCE);
		$.cookies.set( COOKIE_DEFAULT_VIEWS_INSTANCE, defaultViews_Ins );		
	}	
	
	ASSET_TYPES_VIEWS = [];
	ASSET_TYPES_VIEWS_INS = [];

	if ((defaultViews_Def !== null) && (associativeArrayLength(defaultViews_Def) > 0)){
		$.each(defaultViews_Def, function(index, item){
			ASSET_TYPES_VIEWS[item['Key']] = item['Value'];
		});
	}
	
	if ((defaultViews_Ins !== null) && (associativeArrayLength(defaultViews_Ins) > 0)){
		$.each(defaultViews_Ins, function(index, item){
			ASSET_TYPES_VIEWS_INS[item['Key']] = item['Value'];
		});
	}
		
}

function onRunAsUserChanged(ev){
	var e = ev || event;
   // now you can get to the event via evt on any browser.	
    if (e.keyCode == 13) { onRunAsSelectionMade();	 }	
}

function onRunAsSelected(){

	activateRunAsAutoComplete();

	$("#runAsDialog").dialog({
		width: 450,
		height: 125,
		modal: true,
		autoOpen: false,
		open:function() {
		    	$(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
				USER_ID_RUN_AS = USER_ID_ACTUAL;
				var displayName = user_GetDisplayName(USER_ID_ACTUAL);
				$("#lstUsers").val(displayName);
				$("#lstUsers-id").val(USER_ID_ACTUAL);
				$("#lstUsers").select();
			},
		buttons: {
			Cancel: function() {
				$("#runAsDialog").dialog('close');
			},
			Ok: function() {
				onRunAsSelectionMade();				
			}
		}
	});
		
	$("#runAsDialog").dialog('open');	
}

function onRunAsSelectionMade(){
	//var selectedId = $('#lstUsers-id').val();
	var selectedId = USER_ID_RUN_AS;
	var currentActualId = $.cookies.get(COOKIE_UID);
	
	if (selectedId == currentActualId){
		selectedId = null; // clear it out
		IS_RUN_AS = false;
		USER_ID_RUN_AS = selectedId;
	}
	else {
		IS_RUN_AS = true;
		USER_ID_RUN_AS = selectedId;
	}
	
	CURRENT_USER = getUser(selectedId);
	
	$.cookies.set( COOKIE_DEFAULT_VIEWS_DEFINITION, null );
	$.cookies.set( COOKIE_DEFAULT_VIEWS_INSTANCE, null );
	$.cookies.set( COOKIE_BOOKMARKS, null );
	$.cookies.set( COOKIE_VIEWS, null );
	$.cookies.set( COOKIE_PROCESSES, null );
	$.cookies.set( COOKIE_RUN_AS, selectedId );
	
	$("#runAsDialog").dialog('close');
	showMessage('Refresh Needed', 'The page will now be refreshed', true, 400, false);
	location.reload();	
}

function onSearchPhraseChanged(ev){
	var e = ev || event;
   // now you can get to the event via evt on any browser.	
    if (e.keyCode == 13) { onSearch(); }	
}

function onSearch(){

	var userId = USER_ID_ACTUAL;
	if ((IS_RUN_AS) && (USER_ID_RUN_AS !== null)){
		userId = USER_ID_RUN_AS;
	}
	
	var valueToSearch = $('#txtSearchField').val();
	var newUrl = URL_CLIENT_UI + '/search.html' + '?phrase=' + valueToSearch + '&userId=' + userId;
	window.location = newUrl;
}

function pausecomp(millis) {
	var date = new Date();
	var curDate = null;
	
	do { curDate = new Date(); } 
	while(curDate-date < millis);
} 
	
function selectOption_GetCurrentText(controlId) {
	var controlName = '#' + controlId;
	return $(controlName + ' option[value="' + $(controlName).val() + '"]').text();
}

function selectOption_GetInsertionIndex(newValue, controlId) {
	
	var index = 0;
	var indexFound = false;
	var hasItems = false;

	$('#' + controlId + ' > option').each(function() {
		
		hasItems = true;
		var tempVal = $(this).val();
		var tempText = $(this).text().toLowerCase();
		
		if ((tempVal != '0') && (tempVal != '-1'))
		{
			if (newValue.toLowerCase() < tempText ) { 
				// weird way of breaking out of a loop, i know.
				// blame it on jQuery, not me
				// http://gavinroy.com/jquery-tip-how-to-break-out-of-each
				indexFound = true;
				return false; 			
			}
		}
		
		index++;

	});
	
	if (indexFound) {
		return index;
	}
	else {
		if (hasItems){
			return (index - 1);
		}
		else {
			return -1;	
		}		
	}
	
}

function selectOption_GetIndexByValue(controlId, optionValue) {
	var controlName = '#' + controlId;
	var found = false;
	var index = 0;
	
	optionValue = optionValue.toLowerCase();
	
	$(controlName + ' > option').each(function() {
		
		var tempVal = $(this).val().toLowerCase();
		
		if (optionValue == tempVal ) { 
			found = true;
			// weird way of breaking out of a loop, i know.
			// blame it on jQuery, not me
			// http://gavinroy.com/jquery-tip-how-to-break-out-of-each
			return false; 			
		}
		
		index++;

	});
	
	if (found) {
		$(controlName + ' option:eq(' + index + ')').attr("selected", "selected");		
		return index;
	}
	else {
		return -1;
	}	
}

function selectOptionsRemove(controlId) {
	$('#' + controlId)
	    .find('option')
	    .remove()
	    .end();	
}

function selectOptionText(controlId, value) {
	
	// ***
	// Gets the select option text for the specified value	
	// ***
	
	var controlName = '#' + controlId;
	return $(controlName + ' option[value="' + value + '"]').text();
}

function showFullImage(imageId, imageName, requestUrl){
	
	var srcUrl = SVC_IMAGE_GET_THUMBNAIL + '?imageId=' + imageId + '&createThumbnail=false&maxSize=0';
	var html = [];
	
	html.push('<img src=\"' + srcUrl + '\"/>');
	if ((requestUrl !== null) && (requestUrl != undefined) && (requestUrl != '')){
		html.push('<br/>' + requestUrl);
	}
	
	var dimensions = image_GetDimensions(imageId);
	
	var imgW = 200; // default dimensions for image dialog
	var imgH = 200;
	
	if ((dimensions !== null) && (dimensions !== '') && (dimensions != '0x0')) {
		var temp = dimensions.split('x');
		imgW = parseInt(temp[0],10) + 75;
		imgH = parseInt(temp[1],10) + 90;
		var frameW = parseInt($(this).width(),10);
		var frameH = parseInt($(this).height(),10);
		if ((imgW > frameW) || (imgH > frameH))
		{
			imgW = frameW - 25;
			imgH = frameH - 25;
		}
	}
	
	$('#messageDialog').dialog('destroy');
	$('#messageDialog').html(html.join(''));
	$("#messageDialog").dialog({ 
		width: 'auto',
		height: imgH,
		modal: true,
		autoOpen: false,
		resizable: true,
		draggable: true,
		title: imageName + '<br/>(' + imgW + 'x' + imgH + ')'
	});	
	$('#messageDialog').dialog('open');		
}

function showMessage(title, textToShow, isModal, widthInPx, showOkButton){	

	if (widthInPx == null) { widthInPx = 400; }
	if (showOkButton == null) { showOkButton = true; }

	//var windowHeight = determineDialogHeight(textToShow);
	$('#messageDialog').dialog('destroy');
	$("#messageDialog").html(textToShow);
	
	if ((showOkButton !== null) && (showOkButton)){
		$("#messageDialog").dialog({
			width: widthInPx,
			height: 'auto',
			autoOpen: false,
			open:function() {
			    	$(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
				},
			buttons: {
				Ok: function() {
					$(this).dialog('close');
				}
			},			
			modal: isModal,
			title: title,
			position: 'center',
			closeOnEscape: true
		});		
	}
	else {
		$("#messageDialog").dialog({
			width: widthInPx,
			height: 'auto',
			autoOpen: false,
			open:function() {
			    	$(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
				},			
			modal: isModal,
			title: title,
			position: 'center',
			closeOnEscape: true
		});		
	}

	
	$('#messageDialog').dialog('open');
}

function splitCurrency(symbolAndAmount){

	// takes a formatted currency value and splits it into an array
	// with the currency symbol in the zero location
	// and the amount in the one index

	var intLoc = -1;
	$.each(symbolAndAmount, function(index, value) { 
	  	if (parseInt(value)){
	  		intLoc = index;
	  		return false;
	  	}
	});
	
	var values = [];

	if (intLoc != -1){
		values[0] = symbolAndAmount.substr(0, intLoc);
		values[1] = symbolAndAmount.substr(intLoc, symbolAndAmount.length - intLoc + 1);
	}
	
	return values;
}

function toggleButtons(buttonClass, isEnabled) {
	if (isEnabled) {
		$('.' + buttonClass).removeAttr('disabled');
		$('.' + buttonClass).removeClass('ui-state-disabled').addClass('ui-state-default');	
	}
	else {
		$('.' + buttonClass).attr('disabled', 'disabled');
		$('.' + buttonClass).removeClass('ui-state-default').addClass('ui-state-disabled');		
	}
}

function toggleControl(controlId, isEnabled, addClass){
	if (addClass == undefined) { addClass = true; }
	var targetControlId = '#' + controlId;
	if (isEnabled){
		$(targetControlId).removeAttr('disabled');
		if (addClass) { 
			$(targetControlId).removeClass('ui-state-disabled').addClass('ui-state-default');			
		}
		else {
			$(targetControlId).removeClass('ui-state-disabled')	
		}
	}
	else {
		$(targetControlId).attr('disabled', 'disabled');
		if (addClass) { $(targetControlId).removeClass('ui-state-default').addClass('ui-state-disabled'); }
	}	
}