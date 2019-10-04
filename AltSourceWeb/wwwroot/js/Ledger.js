var app = angular.module("root", ["ngResource"]);
app.controller("ledger", ["$scope", "$resource", "$http", "$rootScope", function ($scope, $resource, $http, $rootScope) {

    $scope.isLoggedIn = false;
    $scope.userID = "";
    $scope.password = "";

    $scope.logIn() = function () {
        alert($scope.userID);
    };

    $scope.init = function () {
        alert("init");
    };

    $scope.init();
}])