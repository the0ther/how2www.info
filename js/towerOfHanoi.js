(function () {
    var numDisks = 4;
    var board = [ 
      {"vals": [], "name": "peg1" }, 
      {"vals": [], "name": "peg2" },
      {"vals": [], "name": "peg3" }
    ];
    
    function towerOfHanoi(size) {
      for (var ii = size; ii > 0; ii--) {
        board[0].vals[ii % size] = "disc: " + (size - (ii % size));
      }
    
      printBoard(board);
      move(size, board[0], board[2], board[1]);
      console.log('finished');
      printBoard(board);
    }
    
    function move(n, source, dest, spare) {
      if (n === 1) {
        var disc = source.vals.pop();
        dest.vals.push(disc);
        //console.log('Move top disc ' + disc + ' from ' + source.name + ' to ' + dest.name);
        printBoard(board);
      } else {
        move(n-1, source, spare, dest);
        var disc = source.vals.pop();
        dest.vals.push(disc);
        //console.log('Move top disc ' + disc + ' from ' + source.name + ' to ' + dest.name);
        printBoard(board);
        move(n-1, spare, dest, source);
      }
    }
    
    function printBoard(board) {
      console.log('[[' + board[0].vals + '],[' + board[1].vals + '],[' + board[2].vals + ']]');
    }
    
    towerOfHanoi(numDisks);
})();