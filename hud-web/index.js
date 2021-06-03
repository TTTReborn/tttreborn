// Viewlayer
let active
let scoreboard = document.querySelector(".scoreboard")
let shop = document.querySelector(".shop")
let inspect = document.querySelector(".inspect")
let winscreen = document.querySelector(".winscreen")

let overlayFunctions = {
    scoreboard: (e) => {
        e.preventDefault()
        if (!active) {
            scoreboard.className = "scoreboard fixed open"
            active = true
            let listener = document.addEventListener("keyup", e2 => {
                if (e2.code == "Tab") {
                    scoreboard.className = "scoreboard fixed"
                    document.removeEventListener("keyup", listener)
                    active = undefined
                }
            })
        }
    },
    shop: (e) => {
        console.log(active)
        if (!active) {
            shop.className = "shop fixed open"
            active = true
            let listener = document.addEventListener("keyup", e2 => {
                if (e2.code == "KeyB") {
                    shop.className = "shop fixed"
                    document.removeEventListener("keyup", listener)
                    active = undefined
                }
            })
        }
    },
    inspect: (e) => {
        if (!active) {
            inspect.className = "inspect fixed open"
            active = true
            let listener = document.addEventListener("keyup", e2 => {
                if (e2.code == "KeyC") {
                    inspect.className = "inspect fixed"
                    document.removeEventListener("keyup", listener)
                    active = undefined
                }
            })
        }
    },
    winscreen: (e) => {
        e.preventDefault()
        if (!active) {
            winscreen.className = "winscreen fixed open"
            active = true
            let listener = document.addEventListener("keyup", e2 => {
                if (e2.code == "Escape") {
                    winscreen.className = "winscreen fixed"
                    document.removeEventListener("keyup", listener)
                    active = undefined
                }
            })
        }
    }
}

document.addEventListener('keydown', (e) => {
    if (e.code == "Tab") overlayFunctions.scoreboard(e)
    else if (e.code == "KeyB") overlayFunctions.shop(e)
    else if (e.code == "KeyC") overlayFunctions.inspect(e)
    else if (e.code == "Escape") overlayFunctions.winscreen(e)
});


// Shop
let activeItem
document.querySelectorAll(".shop .item").forEach(item => {
    item.onclick = (e) => {
        const eItem = e.target.closest(".item-holder")
        if (eItem.className.includes("active")) return
        if (activeItem) activeItem.className = 'item-holder'
        eItem.classList.add("active")
        activeItem = eItem
    }
})


// Scoreboard

let scoreboardContent = document.querySelector(".scoreboard main .scoreboard-holder")
let usernames = ["sonsausage", "realtorgas", "besiegeimmediate", "laborerbiological", "glassesburritos", "golfadolescent", "closetragefilled", "miserableoption", "lepecajole", "tragicgarboard", "consultantsplendid", "salarybellbottoms", "armedthey", "lecternfemale", "topmastbrag", "initiativecommunity", "manongoing", "wrestleglide", "guillemotpursue", "cheapsystem", "welshnotice", "hundredwardroom", "almondsharmful", "quicklyunbreaking", "drunkarddray", "unbalancedamazing", "beingpiston", "creationpigstep", "taboonightgown", "gargantuansubdued", "leavescerebrum", "frostyour", "motionlessbecome", "huttrackball", "columninjury", "cheesesteakgerbil", "fortunepod", "supportwastes", "tippedprincipal", "boltchick", "estimatorscooter", "seagulltime", "birdstock", "bureaucratzap", "referencequack", "greaterneigh", "rabbitsiege", "turnalluring", "confrontcustom", "becausesong", "mildlabored", "postboozer", "restlessjerk", "omniscientchough", "sockbucket", "guffawshell", "garagehoe", "urethralater", "vivaciousemigrate", "mendingmean", "draworange", "frisbeeregretful", "companywashboard", "floataspiring", "freshtea", "streetswarm", "rissolesran", "ashamedsorry", "gatherdirt", "softwarelack", "obsessedinstant", "oleanderastronomer", "councilbetter", "irateburgee", "polentaadventure", "lutediaphragm", "levercloser", "somethingagressive", "kookaburramechanic", "windsurfermuffin", "parcelstaid"]
let entriesToAdd = 22

let scoreboardGroups = ["alive", "dead", "spectator"]
let playerCounter = 0

let allPlayers = []
scoreboardGroups.map((e, i) => {
    let players = getPlayers(2 + Math.floor(Math.random() * 8))
    let group = createGroup(e)
    if (playerCounter += players.length) {
        players.forEach(player => {
            // player.type = e 
            addGroupEntry(group, player)
        })
        group.firstElementChild.innerHTML += " - " + players.length
        group.querySelector(".content .headerline:first-child").className += i == 0 ? " traitor" : i == 1 ? " dete" : ""
        scoreboardContent.appendChild(group)
        allPlayers.push(...players)
    }
})
document.querySelector(".playerCounter").innerHTML = playerCounter

function getPlayers(amount) {
    let players = Array.from(new Array(amount))
        .map(e => getRandomPlayerData())
        .sort((a, b) => b.karma - a.karma)
    return players
}

function getRandomPlayerData() {
    let username = usernames.shift(Math.floor(Math.random() * usernames.length))
    let values = {
        name: username,
        karma: 1000 - Math.floor(Math.random() * 250),
        score: Math.floor(Math.random() * 50) - 25,
        ping: Math.floor(Math.random() * 200)
    }
    return values
}

function addGroupEntry(group, values) {
    let headerline = createHeaderline(values)
    group.lastElementChild.appendChild(headerline)
}
function createGroup(type) {
    let group = document.createElement("div")
    let label = document.createElement("div")
    label.innerHTML = type
    label.className = "label"
    let content = document.createElement("div")
    content.className = "content"
    group.appendChild(label)
    group.appendChild(content)
    group.className = "scoreboard-group " + type
    return group
}
function createHeaderline(values) {
    let headerline = document.createElement("div")
    let imageHolder = document.createElement("div")
    let image = document.createElement('img')
    image.src = "images/steam_standard.png"
    imageHolder.appendChild(image)
    headerline.appendChild(imageHolder)
    headerline.className = "headerline"
    Object.values(values).forEach(e => {
        let span = document.createElement("span")
        span.innerHTML = e
        headerline.appendChild(span)
    })
    return headerline
}

// Winscreen

function generateWinObj(players, amountWinner) {
    let getRandomIndex = () => Math.floor((Math.random()) * players.length)
    let getRandomPlayer = () => players.splice(getRandomIndex(), 1)

    let winners = []

    while (amountWinner-- > 0) winners.push(...getRandomPlayer())
    let getRandomAmount = () => Math.floor((Math.random()) * (players.length / 4))
    winners.map(winner => {
        let rndAmount = getRandomAmount()
        winner.kills = []
        while (rndAmount-- > 0) winner.kills = [...winner.kills, ...getRandomPlayer()]
    })


    return winners
    function generateTree() {
        let RandomAmount = getRandomAmount()

        i = i || 0
        // if(i==0) 
    }
}

function createTree(args, currentDepth) {
    currentDepth = currentDepth === undefined ? 0 : currentDepth;

    var node = {
        ...player,
        kills: []
    };

    if (currentDepth < args.depth) {
        for (var i = 0; i < args.spread; i++) {
            node.kills.push(createTree(args, currentDepth + 1));
        }
    }
    return node;
}
