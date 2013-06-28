function towerOfHanoi(size) {
    var myBoard = [[],[],[]];
    for (var ii=size;ii>0;ii--) {
        myBoard[0][ii % size] = size - (ii % size) -1;
    }
    console.log(myBoard[0]);
    move(myBoard);
}

function move(board) {
    // keep taking from peg 0 until it is empty
        // use .pop() on the arrays. the pegs are stacks
    console.log(board);
    if (board[0].length > 0 && board[1].length == 0) {
        //if (board[0][board[0].length-1] < )
        console.log('in here');
    }
    // 
}

function canMoveTo(board, dest) {
    
}

towerOfHanoi(3);