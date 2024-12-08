select * from "Users";

select * from "Tasks";

select *
from "Users" users
inner join "Tasks" tasks
on users.user_id = tasks.user_id;