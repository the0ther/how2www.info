var loops = 0;

function towerOfHanoi(size) {
    var myBoard = [[],[],[]];
    for (var ii=size;ii>0;ii--) {
        myBoard[0][ii % size] = size - (ii % size) -1;
    }
    //printBoard(myBoard);
    while (!isSolved(myBoard) && loops < 15) {
       myBoard = move(myBoard, size);
       loops++;
    }
    console.log('finished');
}

function move(board, size) {
/*

1. move n−1 discs from A to B. This leaves disc n alone on peg A
2. move disc n from A to C
3. move n−1 discs from B to C so they sit on disc n

*/
    printBoard(board);
}

function printBoard(board) {
    console.log('[' + board[0] + '],[' + board[1] + '],[' + board[2] + ']]');   
}

function isSolved(board) {
    var prev = -1;
    
    if (board[0].length === 0) {
        if (board[2].length === 0) {
            for (var ii = 0; ii < board[1].length; ii++) {
                if (board[1][ii] !== ii )
                    return false;
            }
            return true;
        } else if (board[1].length === 0) {
            for (var ii = 0; ii < board[2].length; ii++) {
                if (board[2][ii] !== ii )
                    return false;
            }
            return true;            
        }
    }
    return false;
}

function canMoveTo(board, dest) {
    
}

towerOfHanoi(3);