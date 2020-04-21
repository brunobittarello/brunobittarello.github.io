
"use strict"; // Start of use strict

var data;


$(document).ready(function () {
    console.log("ready!");
    loadData();
});

function loadData() {
    $.getJSON("https://gist.githubusercontent.com/brunobittarello/e7a3fabf7a6c2e72d13a69ef492fec11/raw/teste.json", function (json) {
        data = json;
        //console.log(json);
        populateGames(data.games);
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

    if (personal.otherSkills.length < 1) return;
    html += "<ul class=\"text-left\">";
    for (var i = 0; i < personal.otherSkills.length; i++)
        html += "<li>..." + personal.otherSkills[i] + "</li>";
    html += "</ul>";

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

//Games Selection
function populateGames(games) {
    var gamesContainer = $("#gamesContainer");
    var row;
    for (let i in games) {
        if (i % 3 == 0) {
            row = $("<div class=\"row\"></div>");
            gamesContainer.append(row);
        }
        row.append(createGameIcon(i, games[i]));

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
function onGameIconClicked(id) {

    populateGameInfo(data.games[id]);
    $("#portfolioModal").modal();
}

function populateGameInfo(game) {
    $("#gameTitle").text(game.name);
    $("#gameSummary").text(game.summary);
    $("#gameImage").attr("src", game.image);
    $("#gameDate").text(game.date);

    var contPlatforms = $("#gamePlatform");
    contPlatforms.empty();
    contPlatforms.text("Platforms: ");
    for (const platform of game.platforms) {        
        var hasLink = (platform.link !== "");
        var platformHtml = hasLink ? $("#templGamePlatformLink").clone() : $("#templGamePlatform").clone();//TODO find templates only one time
        platformHtml.appendTo(contPlatforms);
        if (hasLink) {
            platformHtml.attr("href", platform.link);
            platformHtml.children(0).text(platform.name)
        }
        else
            platformHtml.text(platform.name)
        platformHtml.removeAttr("id");
    }

    setGamePageButton(game.localPage)
}

function setGamePageButton(localPage) {
    const gamePage = $("#gamePage");
    if (localPage == "") {
        gamePage.addClass("d-none");
        return;
    }
    gamePage.removeClass("d-none");
    gamePage.attr("href", "pages/games.html?" + localPage);
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
