function towerOfHanoi(size) {
    var myBoard = [[],[],[]];
    for (var ii=size;ii>=0;ii--) {
        myBoard[0][ii] = ii % size;
    }
    console.log(myBoard[0]);
}

function move(board) {
    
}

towerOfHanoi(3);