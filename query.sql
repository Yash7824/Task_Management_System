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





SELECT * FROM "Tasks";
SELECT * FROM Users;


CREATE TRIGGER before_insert_user
BEFORE INSERT ON "Users"
FOR EACH ROW
EXECUTE FUNCTION generate_user_id();

truncate table "Users";
truncate table "Tasks";

ALTER TABLE "Users"
ALTER COLUMN user_id TYPE VARCHAR(255);

ALTER TABLE "Tasks" DROP CONSTRAINT fk_user_id;


insert into "Users"("username", "user_email") values('Yash', 'Yash@gmail.com');
ALTER TABLE "Users" RENAME TO users;
ALTER TABLE "Tasks" RENAME TO tasks;






