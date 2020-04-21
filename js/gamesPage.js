
"use strict"; // Start of use strict

var data;

$(document).ready(function () {
    loadData();
});

function loadData() {
    $.getJSON("https://gist.githubusercontent.com/brunobittarello/e7a3fabf7a6c2e72d13a69ef492fec11/raw/teste.json", function (json) {
        data = json;
        loadPage();
    });
}

function loadPage() {
    const gamePageName = getGameSelected()
    if (data != null && gamePageName != "") {
        const game = findGame(gamePageName, data.games);
        if (game != null) {
            loadGame(game)
            return;
        }
    }
    //window.location.replace("http://www.w3schools.com");
}

function getGameSelected() {
    const param = window.location.search;
    if (param.length == 0) return "";

    return param.substr(1);
}

function findGame(game, games) {
    for (var i = 0; i < games.length; i++)
        if (games[i].localPage == game)
            return games[i];
    return null;
}

function loadGame(game) {
    loadHeader(game);
    loaderDescription(game);
    loadPlatforms(game.platforms);
    loadDetails(game.details);
    loadProdution(game.production);
    loadMedia(game.media);
}

function loadHeader(game) {
    $("#gameImage").attr("src", game.image);
    $("#gameName").html(game.name);
    $("#gameSummary").html(game.summary);
}

function loaderDescription(game) {
    var html = "";
    for (var i = 0; i < game.description.length; i++)
        html += getParagrath(game.description[i]);

    $("#gameDescription").html(html);
}

function loadPlatforms(platforms) {
    var html = "";
    for (var i = 0; i < platforms.length; i++)
        html += getPlatformDownloadButton(platforms[i]);
    $("#gamePlatforms").html(html);
}

function loadDetails(details) {
    var genres = "";
    for (var i = 0; i < details.genres.length; i++)
        genres += ", " + details.genres[i];
    $("#gameGenres").html(genres.substr(2));

    if (details.onlinePlayerTotal != "undefined" && details.onlinePlayerTotal > 0)
        $("#gameOnlinePlayers").html(details.onlinePlayerTotal);
    else {
        $("#gameOnlinePlayersLabel").remove();
        $("#gameOnlinePlayers").remove();
    }

    $("#gameLocalPlayers").html(details.localPlayerTotal);
    $("#gameLastUpdate").html(details.lastUpdated);
}

function loadProdution(production) {
    $("#gameMotivation").html(getParagrath(production.motivation));
    const history = $("#gameHistoryContainer");

    for (var i = 0; i < production.history.length; i++)
        buildHistoryRow(production.history[i]).appendTo(history);
}

function buildHistoryRow(history) {
    var row = $("#gameHistoryRowTemplate").clone();
    row.removeAttr("id");
    row.find(".gameHistoryName").html(history.name);
    row.find(".gameHistoryDate").html(history.date);
    row.find(".gameHistoryObjective").html(history.objective);
    return row;
}

function loadMedia(media) {
    const screenshotContainer = $(".carousel-inner");
    const indicators = $(".carousel-indicators");

    for (var i = 0; i < media.screenshots.length; i++) {
        buildScreenshot(media.screenshots[i], i == 0).appendTo(screenshotContainer);
        indicators.append("<li data-target=\"#myCarousel\" data-slide-to=\"" + i + "\" " + (i == 0 ? "class=\"active\"" : "") + "></li>")        
    }

    if (media.videos.length > 0)//TODO change for more than one video
        $("#gameVideo").attr("src", media.videos[0]);
    else
        $(".game-video").remove();
}

function buildScreenshot(screenshot, active) {
    var row = $("#screenshotTemplate").clone();
    row.removeAttr("id");
    row.find("h3").html(screenshot.name);
    row.find("p").html(screenshot.description);
    row.find("img").attr("src", screenshot.link);
    if (active)
        row.addClass("active");
    return row;
}


//HTML
function getParagrath(text) {
    return "<p class=\"section-subhead text-justify paragraph-tab\">" + text + "</p>"
}

function getPlatformDownloadButton(platform) {
    return "<a class=\"paragraph-tab-half\" href=\"" + platform.link + "\" target=\"_blank\"><img src=\"" + getImageByPlatform(platform.name) + "\" class=\"img-responsive\" alt=\"" + platform.name + "\" title=\"" + platform.name + "\"></a>";
}

function getImageByPlatform(platformName) {
    if (platformName == "Android") return "/img/icons/Android-52.png";
    return "/img/icons/WindowsLogo-52.png";
}