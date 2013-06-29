var loops = 0;
var n = 3;

function towerOfHanoi(size) {
    var myBoard = [[],[],[]];
    for (var ii=size;ii>0;ii--) {
        myBoard[0][ii % size] = size - (ii % size) -1;
    }
    //printBoard(myBoard);
    // while (!isSolved(myBoard) && loops < 15) {
    //    myBoard = move(myBoard, size);
    //    loops++;
    // }
    move(myBoard, size, 0, 1);
    console.log('finished');
}

function move(board, size, source, dest) {
/*
1. move n−1 discs from A to B. This leaves disc n alone on peg A
2. move disc n from A to C
3. move n−1 discs from B to C so they sit on disc n
*/

    printBoard(board);
    if (isSolved(board)) {
        return board;
    } else if (board[2][0] === 0) {
        // going from B to C here
        if (board[1][size-1] < board[2][board[2].length-1]) {
            board[2].push(board[1].pop());
            return board;
        }
        move(board, size-1, 1, 2);
    } else if (board[0].length === 1 && board[2].length[0]) {
        board[2].push(board[0].pop());
        move(board, n, 1, 2);
    } else {
        // moving A to B here
        if (board[0][size-1] < board[1][board[1].length-1]) {
            board[1].push(board[1][size-1]);
        }
        move(board, size-1, 0, 1);
    }
    //printBoard(board);
}

function printBoard(board) {
    console.log('[' + board[0] + '],[' + board[1] + '],[' + board[2] + ']]');   
}

function isSolved(board) {
    var prev = -1;
    
    if (board[0].length === 0 && board[2].length === n) {
        for (var ii = 0; ii < board[2].length; ii++) {
            if (board[2][ii] !== ii ) {
                return false;
            }     
        }
        return true;
    }
    return false;
}

towerOfHanoi(n);