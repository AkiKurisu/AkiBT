English:
1. ``Player`` will track ``Enemy`` when it is 4 meters away from ``Enemy``
2. Click the canMove variable in the AIModel script mounted by the ``Player`` to control the movement of the ``Player``
3. ``Enemy`` will randomly find a location to move, and then search again after 5 seconds after arriving
4. ``Patrol`` will randomly find a location to move, and find a new location every 10 seconds
* The above examples all use shared variables for value transfer
* Example2 use ``ScriptableObject`` as behavior tree container

中文：
1. ``Player``会在距离``Enemy``4米外后追踪``Enemy``
2. 点击``Player``挂载的AIModel脚本中的canMove变量来控制``Player``的移动
3. ``Enemy``会随机找位置移动，抵达后过5秒再重新寻找
4. ``Patrol``会随机找位置移动,每10秒找新位置
* 以上样例均使用了共享变量进行值传递
* Example2使用了``ScriptableObject``作为行为树载体