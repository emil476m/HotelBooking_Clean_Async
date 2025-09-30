Feature: CreateBooking
	
	

A way to book a room

@mytag
Scenario: Create a booking sucessfully
	Given a booking starting on <StartDate>
	And ending on <EndDate>
	When the booking is created
	Then the booking should be created sucessfully
	
	Examples: 
	| StartDate | EndDate |
	| 5    		| 7       |
	| 2    		| 3       |
	| 21    	| 25      |