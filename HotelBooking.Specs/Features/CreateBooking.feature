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
 
Scenario: Create a booking unsuccesfully
	Given a booking starting on <StartDate>
	And ending on <EndDate>
	When the booking creation attemt is made
	Then the booking should be created unsucessfully
	
	Examples: 
	  | StartDate | EndDate |
	  | 7         | 5       |
	  | 3         | 2       |
	  | 25        | 21      |
	  | 0         | 2       |
	  | -1        | 5       |

Scenario: Create booking fully occupied
	Given a booking starting on <StartDate>
	And ending on <EndDate>
	When the booking is created
	Then the booking should not be created sucessfully
	
	Examples: 
	  | StartDate | EndDate |
	  | 10        | 12      |
	  | 12        | 15      |
	  | 20        | 25      |
