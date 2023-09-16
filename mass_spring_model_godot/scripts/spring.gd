extends CharacterBody3D

var coordinate = 0
var initial_length = scale.z

func _on_is_moving(coord):
	coordinate = float(coord)
	
func _process(_delta):
	if !(coordinate == 0):
		scale.z = initial_length * -coordinate
