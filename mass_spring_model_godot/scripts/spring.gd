extends CharacterBody3D

var coordinate = 0

var moving_left = false
var moving_right = false
var initial_length = scale.z

func _on_mass_is_moving_right(coord):
	moving_right = true
	moving_left = false
	coordinate = float(coord)
	
func _on_mass_is_moving_left(coord):
	moving_left = true
	moving_right = false
	coordinate = float(coord)
	
func _on_mass_stop_moving():
	moving_left = false
	moving_right = false
	
func _process(_delta):
	if moving_right:
		scale.z = initial_length * coordinate
		moving_right = false
	elif moving_left:
		scale.z = initial_length * -coordinate
		moving_left = false
	else:
		scale.z += 0

