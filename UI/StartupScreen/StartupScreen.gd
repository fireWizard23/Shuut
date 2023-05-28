extends Control



func _on_start_pressed():
	GameManager.GoToMain()

func _on_quit_pressed():
	get_tree().quit()
