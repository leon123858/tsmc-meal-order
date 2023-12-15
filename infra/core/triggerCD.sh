# 定义存储触发器名称的列表
triggers=("mail-trigger" "user-trigger" "menu-trigger" "order-trigger" "storage-trigger" "web-trigger" "ai-trigger")

# 循环触发每个触发器并行执行
for trigger in "${triggers[@]}"; do
  gcloud builds triggers run $trigger --region=asia-east1 --branch=main &
done

# 等待所有后台作业完成
wait
