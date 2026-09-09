//leftmenu toggle
$(document).ready(function () {
    $('.dropdown-bs-toggle').on('click', function (e) {
        $(this).next().toggle();
    });
    $('.dropdown-menu').on('click', function (e) {
        e.stopPropagation();
    });
    if (1) {
        $('body').attr('tabindex', '0');
    }
    else {
        alertify.confirm().set({ 'reverseButtons': true });
        alertify.prompt().set({ 'reverseButtons': true });
    }

    //leftmenu toggle (mobile overlay handled by responsive.js)
    $('.toggle_sidebar').click(function (e) {
        if (window.erpResponsive && window.erpResponsive.isNarrow && window.erpResponsive.isNarrow()) {
            e.preventDefault();
            window.erpResponsive.toggleSidebarOverlay();
            return;
        }
        $('.main_wrap').toggleClass('collapse_sidebar');
    })
})


"use strict";

var input = document.getElementById('input'), // input/output button
    number = document.querySelectorAll('.numbers div'), // number buttons
    operator = document.querySelectorAll('.operators div'), // operator buttons
    result = document.getElementById('result'), // equal button
    clear = document.getElementById('clear'), // clear button
    resultDisplayed = false; // flag to keep an eye on what output is displayed

// adding click handlers to number buttons (guard: calculator may be absent)
if (input && number && number.length) {
for (var i = 0; i < number.length; i++) {
    number[i].addEventListener("click", function (e) {

        // storing current input string and its last character in variables - used later
        var currentString = input.innerHTML;
        var lastChar = currentString[currentString.length - 1];

        // if result is not diplayed, just keep adding
        if (resultDisplayed === false) {
            input.innerHTML += e.target.innerHTML;
        } else if (resultDisplayed === true && lastChar === "+" || lastChar === "-" || lastChar === "x" || lastChar === "/") {
            // if result is currently displayed and user pressed an operator
            // we need to keep on adding to the string for next operation
            resultDisplayed = false;
            input.innerHTML += e.target.innerHTML;
        } else {
            // if result is currently displayed and user pressed a number
            // we need clear the input string and add the new input to start the new opration
            resultDisplayed = false;
            input.innerHTML = "";
            input.innerHTML += e.target.innerHTML;
        }

    });
}

// adding click handlers to operator buttons
for (var i = 0; i < operator.length; i++) {
    operator[i].addEventListener("click", function (e) {

        // storing current input string and its last character in variables - used later
        var currentString = input.innerHTML;
        var lastChar = currentString[currentString.length - 1];

        // if last character entered is an operator, replace it with the currently pressed one
        if (lastChar === "+" || lastChar === "-" || lastChar === "x" || lastChar === "/") {
            var newString = currentString.substring(0, currentString.length - 1) + e.target.innerHTML;
            input.innerHTML = newString;
        } else if (currentString.length == 0) {
            // if first key pressed is an opearator, don't do anything
            console.log("enter a number first");
        } else {
            // else just add the operator pressed to the input
            input.innerHTML += e.target.innerHTML;
        }

    });
}
} // end calculator presence guard

// on click of 'equal' button
//result.addEventListener("click", function () {
//    debugger
//    var inputString = input.innerHTML;


//    var numbers = inputString.split(/\+|\-|\x|\//g); 

    
//    var operators = inputString.replace(/[0-9]|\./g, "").split("");

//    console.log(inputString);
//    console.log(operators);
//    console.log(numbers);
//    console.log("----------------------------");


//    var divide = operators.indexOf("/");
//    while (divide != -1) {
//        numbers.splice(divide, 2, numbers[divide] / numbers[divide + 1]);
//        operators.splice(divide, 1);
//        divide = operators.indexOf("/");
//    }

//    var multiply = operators.indexOf("x");
//    while (multiply != -1) {
//        numbers.splice(multiply, 2, numbers[multiply] * numbers[multiply + 1]);
//        operators.splice(multiply, 1);
//        multiply = operators.indexOf("x");
//    }

//    var subtract = operators.indexOf("-");
//    while (subtract != -1) {
//        numbers.splice(subtract, 2, numbers[subtract] - numbers[subtract + 1]);
//        operators.splice(subtract, 1);
//        subtract = operators.indexOf("-");
//    }

//    var add = operators.indexOf("+");
//    while (add != -1) {
//        // using parseFloat is necessary, otherwise it will result in string concatenation :)
//        numbers.splice(add, 2, parseFloat(numbers[add]) + parseFloat(numbers[add + 1]));
//        operators.splice(add, 1);
//        add = operators.indexOf("+");
//    }

//    input.innerHTML = numbers[0]; // displaying the output

//    resultDisplayed = true; // turning flag if result is displayed
//});

// clearing the input on press of clear
//clear.addEventListener("click", function () {
//    input.innerHTML = "";
//})

//bubble 
$(function () {

    $("#bubble").dxChart({
        dataSource: bubble,
        commonSeriesSettings: {
            type: 'bubble'
        },
        tooltip: {
            enabled: true,
            location: "edge",
            customizeTooltip: function (arg) {
                return {
                    text: arg.point.tag + '<br/>Total Population: ' + arg.argumentText + 'M <br/>Population with Age over 60: ' + arg.valueText + 'M (' + arg.size + '%)'
                };
            }
        },
        argumentAxis: {
            label: {
                customizeText: function () {
                    return this.value + 'M';
                }
            },
            title: 'Total Population'
        },
        valueAxis: {
            label: {
                customizeText: function () {
                    return this.value + 'M';
                }
            },
            title: 'Population with Age over 60'
        },
        legend: {
            position: 'inside',
            horizontalAlignment: 'left',
            border: {
                visible: true
            }
        },
        palette: ["#d2e1e9", "#055a87", "#04003b", "#7f76ff"],
        onSeriesClick: function (e) {
            var series = e.target;
            if (series.isVisible()) {
                series.hide();
            } else {
                series.show();
            }
        },
        "export": {
            enabled: false
        },
        series: [{
            name: 'Europe',
            argumentField: 'total1',
            valueField: 'older1',
            sizeField: 'perc1',
            tagField: 'tag1'
        }, {
            name: 'Africa',
            argumentField: 'total2',
            valueField: 'older2',
            sizeField: 'perc2',
            tagField: 'tag2'
        }, {
            name: 'Asia',
            argumentField: 'total3',
            valueField: 'older3',
            sizeField: 'perc3',
            tagField: 'tag3'
        }, {
            name: 'North America',
            argumentField: 'total4',
            valueField: 'older4',
            sizeField: 'perc4',
            tagField: 'tag4'
        }]
    });
});

var bubble = [{
    total1: 9.5,
    total2: 168.8,
    total3: 127.2,
    older1: 2.4,
    older2: 8.8,
    older3: 40.1,
    perc1: 25.4,
    perc2: 5.3,
    perc3: 31.6,
    tag1: 'Sweden',
    tag2: 'Nigeria',
    tag3: 'Japan'
}, {
    total1: 82.8,
    total2: 91.7,
    total3: 90.8,
    older1: 21.9,
    older2: 4.6,
    older3: 8.0,
    perc1: 26.7,
    perc2: 5.4,
    perc3: 8.9,
    tag1: 'Germany',
    tag2: 'Ethiopia',
    tag3: 'Viet Nam'
}, {
    total1: 16.7,
    total2: 80.7,
    total3: 21.1,
    older1: 3.8,
    older2: 7.0,
    older3: 2.7,
    perc1: 22.8,
    perc2: 8.4,
    perc3: 12.9,
    tag1: 'Netherlands',
    tag2: 'Egypt',
    tag3: 'Sri Lanka'
}, {
    total1: 62.8,
    total2: 52.4,
    total3: 96.7,
    older1: 14.4,
    older2: 4.0,
    older3: 5.9,
    perc1: 23.0,
    perc2: 7.8,
    perc3: 6.1,
    tag1: 'United Kingdom',
    tag2: 'South Africa',
    tag3: 'Philippines'
}, {
    total1: 38.2,
    total2: 43.2,
    total3: 66.8,
    older1: 7.8,
    older2: 1.8,
    older3: 9.6,
    perc1: 20.4,
    perc2: 4.3,
    perc3: 13.7,
    tag1: 'Poland',
    tag2: 'Kenya',
    tag3: 'Thailand'
}, {
    total1: 45.5,
    total3: 154.7,
    total4: 34.8,
    older1: 9.5,
    older3: 10.3,
    older4: 7.2,
    perc1: 21.1,
    perc3: 6.8,
    perc4: 20.8,
    tag1: 'Ukraine',
    tag3: 'Bangladesh',
    tag4: 'Canada'
}, {
    total1: 143.2,
    total4: 120.8,
    older1: 26.5,
    older4: 11.0,
    perc1: 18.6,
    perc4: 9.5,
    tag1: 'Russian Federation',
    tag4: 'Mexico'
}];

///// pie chart
$(() => {
    $('#pie').dxPieChart({
        palette: 'bright',
        dataSource: pie,
        legend: {
            orientation: 'horizontal',
            itemTextPosition: 'right',
            horizontalAlignment: 'center',
            verticalAlignment: 'bottom',
            columnCount: 4,
        },
        export: {
            enabled: false,
        },
        palette: ["#d2e1e9", "#055a87", "#04003b", "#7f76ff"],
        series: [{
            argumentField: 'country',
            valueField: 'medals',
            label: {
                visible: true,
                font: {
                    size: 16,
                },
                connector: {
                    visible: true,
                    width: 0.5,
                },
                position: 'columns',
                customizeText(arg) {
                    return `${arg.valueText} (${arg.percentText})`;
                },
            },
        }],
    });
});

const pie = [{
    country: 'USA',
    medals: 110,
}, {
    country: 'China',
    medals: 100,
}, {
    country: 'Russia',
    medals: 72,
}, {
    country: 'Britain',
    medals: 47,
}, {
    country: 'Australia',
    medals: 46,
}, {
    country: 'Germany',
    medals: 41,
}, {
    country: 'France',
    medals: 40,
}, {
    country: 'South Korea',
    medals: 31,
}];

////// dougnet
$(() => {
    $('#pie2').dxPieChart({
        type: 'doughnut',
        palette: 'Soft Pastel',
        dataSource: pie2,
        tooltip: {
            enabled: true,
            format: 'millions',
            customizeTooltip(arg) {
                return {
                    text: `${arg.valueText} - ${(arg.percent * 100).toFixed(2)}%`,
                };
            },
        },
        palette: ["#d2e1e9", "#055a87", "#04003b", "#7f76ff"],
        legend: {
            horizontalAlignment: 'center',
            verticalAlignment: 'bottom',
            margin: 0,
        },
        export: {
            enabled: false,
        },
        series: [{
            argumentField: 'region',
            label: {
                visible: true,
                format: 'millions',
                connector: {
                    visible: true,
                },
            },
        }],
    });
});

const pie2 = [{
    region: 'Asia',
    val: 4119626293,
}, {
    region: 'Africa',
    val: 1012956064,
}, {
    region: 'Northern America',
    val: 344124520,
}, {
    region: 'Latin America and the Caribbean',
    val: 590946440,
}, {
    region: 'Europe',
    val: 727082222,
}, {
    region: 'Oceania',
    val: 35104756,
}];

//// bar chart
$(() => {
    $('#bar').dxChart({
        dataSource: bar,
        legend: {
            visible: false,
        },
        series: {
            type: 'bar',
        },
        palette: ["#04003b"],
        argumentAxis: {
            tickInterval: 10,
            label: {
                format: {
                    type: 'decimal',
                },
            },
        },
    });
});

const bar = [{
    arg: 1960,
    val: 3032019978,
}, {
    arg: 1970,
    val: 3683676306,
}, {
    arg: 1980,
    val: 4434021975,
}, {
    arg: 1990,
    val: 5281340078,
}, {
    arg: 2000,
    val: 6115108363,
}, {
    arg: 2010,
    val: 6922947261,
}, {
    arg: 2020,
    val: 7795000000,
}];


//// radar
$(function () {
    $("#radarchart").dxPolarChart({
        dataSource: radarchart,
        useSpiderWeb: true,
        series: [{ valueField: "apples", name: "Apples" },
        { valueField: "grapes", name: "Grapes" },
        { valueField: "lemons", name: "Lemons" },
        { valueField: "oranges", name: "Oranges" }],
        commonSeriesSettings: {
            type: "line"
        },
        "export": {
            enabled: false
        },
        legend: {
            verticalAlignment: "bottom",
            horizontalAlignment: "center"
        },
        tooltip: {
            enabled: true
        }
    });
});

var radarchart = [{
    arg: "USA",
    apples: 4.21,
    grapes: 6.22,
    lemons: 0.8,
    oranges: 7.47
}, {
    arg: "China",
    apples: 3.33,
    grapes: 8.65,
    lemons: 1.06,
    oranges: 5
}, {
    arg: "Turkey",
    apples: 2.6,
    grapes: 4.25,
    lemons: 0.78,
    oranges: 1.71
}, {
    arg: "Italy",
    apples: 2.2,
    grapes: 7.78,
    lemons: 0.52,
    oranges: 2.39
}, {
    arg: "India",
    apples: 2.16,
    grapes: 2.26,
    lemons: 3.09,
    oranges: 6.26
}];

//area
$(function () {
    var chart = $("#area").dxChart({
        dataSource: area,
        commonSeriesSettings: {
            type: "area",
            border: { visible: true }
        },
        series: [
            { valueField: "production", name: "production", color: "#055a87" },
            { valueField: "waste", name: "waste", color: "#04003b" }
        ],

        argumentAxis: {
            valueMarginsEnabled: false
        },
        valueAxis: {
            axisDivisionFactor: 50
        },
        legend: {
            verticalAlignment: "bottom",
            horizontalAlignment: "center"
        }
    }).dxChart("instance");

});

var area = [{
    arg: new Date(2019, 0, 1, 16, 14),
    production: 380,
    waste: null
}, {
    arg: new Date(2019, 0, 1, 17, 4),
    production: 380,
    waste: null
}, {
    arg: new Date(2019, 0, 1, 17, 5),
    production: 220,
    waste: null
}, {
    arg: new Date(2019, 0, 1, 17, 6),
    production: 220,
    waste: null
}, {
    arg: new Date(2019, 0, 1, 17, 8),
    production: 220,
    waste: null
}, {
    arg: new Date(2019, 0, 1, 17, 8),
    production: null,
    waste: 220
}, {
    arg: new Date(2019, 0, 1, 17, 12),
    production: null,
    waste: 220
}, {
    arg: new Date(2019, 0, 1, 17, 12),
    production: 220,
    waste: null
}, {
    arg: new Date(2019, 0, 1, 17, 13),
    production: 580,
    waste: null
}, {
    arg: new Date(2019, 0, 1, 17, 24),
    production: 580,
    waste: null
}];

var types = ["area", "stackedarea", "fullstackedarea"];