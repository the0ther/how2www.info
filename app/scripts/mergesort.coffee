define 'Mergesort', [], () ->
  class Mergesort
    sort: (@list) ->
      atoms = for atom in list
        [ atom ]
      console.log atoms