SELECT * FROM "Users";
SELECT * FROM "Users" WHERE user_id = 'f578155e-1c48-4f92-979b-322d08fff6b6';

UPDATE "Users"
SET username = 'Yash Raj', user_email = 'yashr7824@gmail.com', mobile_num = '9867795734'
WHERE user_id = 'f578155e-1c48-4f92-979b-322d08fff6b6'

SELECT * FROM "Tasks";

SELECT *
FROM "Users" users
INNER JOIN "Tasks" tasks
ON users.user_id = tasks.user_id;