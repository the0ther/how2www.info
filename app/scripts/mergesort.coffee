define 'Mergesort', [], () ->
  class Mergesort
    sort: (@list) ->
      left = for atom in list[0..((list.length/2)-1)]
        [ atom ]
      right = for atom in list[((list.length/2))..]
        [ atom ]
      console.log 'left: ', left, 'right', right
      # pivot 
      # for atom in atoms
