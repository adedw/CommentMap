var builder = DistributedApplication.CreateBuilder(args);


var rabbitUsername = builder.AddParameter("rabbit-username");
var rabbitPassword = builder.AddParameter("rabbit-password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", rabbitUsername, rabbitPassword)
    .WithImageTag("4.3.4")
    .WithManagementPlugin()
    .WithDataVolume("messaging-data");


var pgUsername = builder.AddParameter("pg-username");
var pgPassword = builder.AddParameter("pg-password", secret: true);

var postgres = builder.AddPostgres("postgres", pgUsername, pgPassword)
    .WithImage("postgis/postgis", "18-3.6")
    .WithPgAdmin(o => o.WithImageTag("9.17"))
    .WithDataVolume("comment-map-data");
var commentMapDb = postgres.AddDatabase("comment-map");


var mailpit = builder.AddMailPit("mailpit")
    .WithImageTag("v1.30")
    .WithDataVolume("mailpit-data");


builder.AddProject<Projects.CommentMap_EmailSender>("email-sender")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(mailpit)
    .WaitFor(mailpit);


builder.AddProject<Projects.CommentMap_Mvc>("mvc")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(commentMapDb)
    .WaitFor(commentMapDb);


builder.Build().Run();
