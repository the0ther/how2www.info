function towerOfHanoi(size) {
    var myBoard = [[],[],[]];
    for (var ii=size;ii>0;ii--) {
        myBoard[0][ii % size] = size - (ii % size) -1;
    }
    //printBoard(myBoard);
    move(myBoard, size);
}

function move(board, size) {
    // push to move top of 0 to peg 1
    
    // push to move top of 0 to peg 2
    
    // try to clear peg 2 for top of 0
    
    
    
    
    
    
    // keep taking from peg 0 until it is empty

    
    
    
    printBoard(board);
    
    //var topOfZeroLT
    if (board[0][board[0].length-1] < board[1][board[1].length-1]) {
        // compare top of 0 with top of 1, move if can
        console.log('moving top of 0 to top of 1');
        board[i].push(board[0].pop());
    } else if (board[0][board[0].length-1] < board[2][board[2].length-1]) {
        // compare top of 0 with top of 2, move if can
        console.log("moving top of 0 to top of 2");
        board[2].push(board[0].pop());
    } else if (board[1][board[1].length-1] < board[2][board[2].length-1]) {
        // compare top of 1 with top of 2, move if can
        console.log("moving top of 1 to top of 2");
        board[2].push(board[1].pop());
    } else if (board[2][board[2].length-1] < board[0][board[0].length -1 ]) {
        // compare top of 2 with top of 0, move if can
        board[0].push(board[2].pop());
        console.log("moving top of 2 to top of 0");
    }
    // move top of 2 to top of 1 
    
    // move top of 0 to 1
    
    // move top of 0 to 2
    
    //printBoard(board);
    console.log('size: ', size);
    // if (board[0].length === (size) && board[1].length === 0) {
    //     //if (board[0][board[0].length-1] < )
    //     console.log('in here');
    // }
     
}

function printBoard(board) {
    console.log('[' + board[0] + '],[' + board[1] + '],[' + board[2] + ']]');   
}

function canMoveTo(board, dest) {
    
}

towerOfHanoi(3);