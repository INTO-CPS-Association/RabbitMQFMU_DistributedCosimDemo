extends CharacterBody3D

const SPEED = 2

signal is_moving_right(coord)
signal is_moving_left(coord)
signal stop_moving

var got_message = false
var same_position = false
var coord = []
var t = 0.0
var new_pos : Vector3

func _physics_process(_delta):
	if got_message:
		t = 0.4
		position = position.lerp(new_pos, t)
		
		if !same_position:
			if float(coord[2]) < 0: 
				emit_signal("is_moving_right", coord[2])
			elif float(coord[2]) > 0:
				emit_signal("is_moving_left", coord[2])
			else:
				emit_signal("stop_moving")
		
		same_position = true

func _on_position_message(coordinates):
	got_message = true
	same_position = false
	coord = coordinates
	new_pos = Vector3(position.x + float(coord[0]), 
					  position.y + float(coord[1]), 
					  position.z + float(coord[2]))
