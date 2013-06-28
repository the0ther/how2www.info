function towerOfHanoi(size) {
    var myBoard = [[],[],[]];
    for (var ii=size;ii>0;ii--) {
        myBoard[0][ii % size] = size - (ii % size) -1;
    }
    console.log(myBoard[0]);
}

function move(board) {
    
}

function canMoveTo(board, dest) {
    
}

towerOfHanoi(3);