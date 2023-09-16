extends CharacterBody3D

signal is_moving(coord)

var got_message = false
var coord = 0.0
var t = 0.4
var new_pos : Vector3

func _physics_process(_delta):
	if got_message:
		position = position.lerp(new_pos, t)
		emit_signal("is_moving", coord)

func _on_position_message(coordinates):
	got_message = true
	coord = coordinates
	new_pos = Vector3(position.x + 0, 
					  position.y + 0, 
					  coord * 25)
