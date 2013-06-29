var loops = 0;
var numDisks = 4;
//var p1 = [], p2 = [], p3 = [];
var board = [[], [], []];

function towerOfHanoi(size) {
  for (var ii = size; ii > 0; ii--) {
    board[0][ii % size] = { "originalIndex": ii % size, "value": size - (ii % size) };
  }

  move(size, board[0], board[2], board[1]);
  console.log('finished');
}

function move(n, p1, p2, p3) {
  console.log('n: ', n);
  console.log('board@0: ', board[0], ' board@1: ', board[1], ' board@2: ', board[2]);
  console.log('p1: ', p1, ' p2: ', p2, ' p3: ', p3);
  printBoard(board);
  console.log('');
  if (n > 1) {
    move(n-1, p1, p3, p2);
  }
  if (n === 1) {
    p2.push(p1.pop());
    //console.log("Move top disc from %d to %d.\n", p1, p2);
  }
  if (n > 1) {
    move(n-1, p3, p2, p1);
  }
}

function printBoard(board) {
  console.log('[[' + board[0] + '],[' + board[1] + '],[' + board[2] + ']]');
}

function isSolved(board) {
  if (board[0].length === 0 && board[2].length === n) {
    for (var ii = 0; ii < board[2].length; ii++) {
      if (board[2][ii] !== ii) {
        return false;
      }
    }
    return true;
  }
  return false;
}

towerOfHanoi(numDisks);