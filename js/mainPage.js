
"use strict"; // Start of use strict

var data;
var spinner;

$(document).ready(function () {
    console.log("ready!");
    loadData();
    spinner = $("#portfolioModalSpinner");
    $("#gameImage").on("load", onModalImageLoaded);
});

function loadData() {
    $.getJSON("https://raw.githubusercontent.com/brunobittarello/brunobittarello.github.io/master/data/mainPage.json", function (json) {
        data = json;
        //console.log(json);
        populateGames(data.games);
        populateSoftwares(data.softwares);
        popualteSkillSelections(data.skillSections);
    });
}


//Personal Info
function onPersonalInfoClicked(id) {
    var html = "";
    if (id == 0) html = loadPersonal(data.personal);
    if (id == 1) html = loadEducation(data.education);
    if (id == 2) html = loadProfessional(data.professional);

    if (html != "") {
        var infoData = $("#infoData");
        infoData.empty();
        infoData.append(html);
        $("#personalModal").modal();
    }
}

function loadPersonal(personal) {
    $("#infoTitle").text("Personal");

    var html = "";
    html += createTitle("Objective");
    html += createText(personal.objective);
    html += "<br />";

    html += createTitle("Summary");
    html += createParagraphText(personal.summary);
    html += "<br />";

    html += createTitle("Bio");
    html += createParagraphText(personal.bio);
    html += "<br />";

    html += createTitle("Other Skills");
    html += createParagraphText(personal.otherSkills);
    html += "<br />";

    return html;
}

function loadEducation(education) {
    $("#infoTitle").text("Education");

    var html = "";
    for (var i = 0; i < education.length; i++) {
        html += createTitle(education[i].level);
        for (const course of education[i].courses)
            html += createText("(" + course.period + ") - " + course.description + " (" + course.school + ") (" + course.place + ").");
        html += "<br />";
    }

    return html;
}

function loadProfessional(professional) {
    $("#infoTitle").text("Professional");

    var html = "";
    for (const job of professional) {
        html += createTitle(job.name + " (" + job.date + ")");
        html += createParagraphText("(" + job.place + ") " + job.description);
        html += "<br />";
    }

    return html;
}

function createTitle(text) {
    return "<h3>" + text + "</h3>";
}

function createText(text) {
    return "<p>" + text + "</p>";
}

function createParagraphText(text) {
    return "<p class=\"text-justify paragraph-tab\">" + text + "</p>";
}

//Software Selection
function populateSoftwares(softwares) {
    var softwaresContainer = $("#softwaresContainer");
    for (let i in softwares) {
        softwaresContainer.append(createSoftwareIcon(i, softwares[i]));
    }
}

function createSoftwareIcon(id, game) {
    var html = $("#templGameIcon").clone();
    html.removeAttr("id");
    html.click(function () { onSofwareIconClicked(id); });
    html.children(0).children(0).attr("src", game.icon);

    return html;
}

//Games Selection
function populateGames(games) {
    var gamesContainer = $("#gamesContainer");
    for (let i in games) {
        gamesContainer.append(createGameIcon(i, games[i]));
    }
}

function createGameIcon(id, game) {
    var html = $("#templGameIcon").clone();
    html.removeAttr("id");
    html.click(function () { onGameIconClicked(id); });
    html.children(0).children(0).attr("src", game.icon);

    return html;
}

//Game Info modal
function onSofwareIconClicked(id) {
    populateGameInfo(data.softwares[id]);
    $("#portfolioModal").modal();
}

function onGameIconClicked(id) {
    populateGameInfo(data.games[id]);
    $("#portfolioModal").modal();
}

function populateGameInfo(game) {
    $("#gameTitle").text(game.name);
    $("#gameSummary").text(game.summary);
    spinner.show();
    $("#gameImage").hide();
    $("#gameImage").attr("src", game.image);
    $("#gameDate").text(game.date);

    var contPlatforms = $("#gamePlatform");
    contPlatforms.empty();
    contPlatforms.text("Platforms: ");
    for (const platform of game.platforms) {
        var hasLink = (platform.link != null && platform.link != "");
        var platformHtml = hasLink ? $("#templGamePlatformLink").clone() : $("#templGamePlatform").clone();//TODO find templates only one time
        platformHtml.appendTo(contPlatforms);
        if (hasLink) {
            platformHtml.attr("href", platform.link);
            platformHtml.children(0).text(platform.name);
        }
        else {
            platformHtml.text(platform.name);
            contPlatforms.append(" ");
        }
        platformHtml.removeAttr("id");
    }

    if (game.externalPage)
        setPageButton(game.externalPage, true);
    else
        setPageButton(game.localPage, false);
}

function onModalImageLoaded() {
    $(this).show();
    spinner.hide();
}

function setPageButton(link, external) {
    const gamePage = $("#gamePage");
    if (link == "") {
        gamePage.addClass("d-none");
        return;
    }
    gamePage.removeClass("d-none");
    if (external)
        gamePage.attr("href", link);
    else
        gamePage.attr("href", "pages/games.html?" + link);
}

//Skills
function popualteSkillSelections(skillSelections) {
    var skillsContainer = $("#skillsContainer");
    var html = "";
    for (const skillSelec of skillSelections)
        html += createSkillSection(skillSelec);
    skillsContainer.append(html);
}

function createSkillSection(skillSec) {
    var html = "<div class=\"row\"> <div class=\"col-lg-12 text-center section-header\"> <h1>" + skillSec.name + "</h1> </div></div>";

    for (const skill of skillSec.skills)
        html += createSkill(skill.name, skill.level);

    return html;
}

function createSkill(name, level) {
    var html = "<div class=\"row text-center offset-sm-3\"> <div class=\"col-sm-4\"> <h4>" + name + "</h4> </div>";
    html += "<div class=\"col-sm-4\">";

    for (var i = 0; i < 5; i++)
        html += skillPoint(i < level);

    html += "</div></div>";
    return html;
}

function skillPoint(on) {
    if (on) return "<img class=\"skillpoint\" src=\"img/portfolio/skillOn.png\">";
    return "<img class=\"skillpoint\" src=\"img/portfolio/skillOff.png\">";

}
