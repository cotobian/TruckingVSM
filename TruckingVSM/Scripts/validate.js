﻿
//validate ngay thang
function validateDatetime(input) {

}

//validate so cont
function validateCntrNo(CntrNo) {
    var chars = {
        "A": "10",
        "B": "12",
        "C": "13",
        "D": "14",
        "E": "15",
        "F": "16",
        "G": "17",
        "H": "18",
        "I": "19",
        "J": "20",
        "K": "21",
        "L": "23",
        "M": "24",
        "N": "25",
        "O": "26",
        "P": "27",
        "Q": "28",
        "R": "29",
        "S": "30",
        "T": "31",
        "U": "32",
        "V": "34",
        "W": "35",
        "X": "36",
        "Y": "37",
        "Z": "38"
    }
    var cntr_split = CntrNo.split('');
    
}

//notify loi
function validateNotify(input) {
    $.notify(input, {
        globalPosition: "top center",
        color: "#fff", background: "#D44950"
    })
}