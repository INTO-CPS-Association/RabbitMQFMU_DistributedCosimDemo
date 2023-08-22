extends CharacterBody3D

const SPEED = 2

signal is_moving_right(coord)
signal is_moving_left(coord)
signal stop_moving

var got_message = false
var same_position = false
var coord = 0.0
var t = 0.0
var new_pos : Vector3

func _physics_process(_delta):
	if got_message:
		t = 0.4
		position = position.lerp(new_pos, t)
		
		if !same_position:
			if float(coord) >= 0.3: 
				emit_signal("is_moving_right", coord)
			elif float(coord) < 0.3:
				emit_signal("is_moving_left", coord)
			else:
				emit_signal("stop_moving")
		
		same_position = true

func _on_position_message(coordinates):
	got_message = true
	same_position = false
	coord = coordinates
	new_pos = Vector3(position.x + 0, 
					  position.y + 0, 
					  coord * 25)
