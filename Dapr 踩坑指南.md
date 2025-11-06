
# 🧭 Dapr 安装与踩坑记录（Docker Standalone 模式）

**环境：** Windows 10/11 + Docker Desktop + PowerShell  
**Dapr 版本：** 1.16.2  
**模式：** Standalone（非 Kubernetes）

---

## 🩵 一、背景

在使用 Dapr 时，官方推荐 `dapr init` 自动初始化容器组件，但在 **启用 Docker Desktop 自带的 Kubernetes** 或 **端口占用场景** 下，会出现启动失败的问题，特别是：

```
failed to bind port 0.0.0.0:2379: address already in use
```

或：
```
failed to create data dir: mkdir data: permission denied
```

本记录总结了从问题排查到最终解决的全过程。

---

## ⚙️ 二、问题一：2379 端口被占用

### 📍 现象
执行：
```powershell
dapr init --runtime-version 1.16.2
```
输出错误：
```
failed to bind port 0.0.0.0:2379: address already in use
```

### 🧩 原因
Docker Desktop 内置 Kubernetes 时，会自动启动以下容器：

| 组件 | 默认端口 |
|------|-----------|
| etcd | 2379（客户端端口） / 2380（peer端口） |

Kubernetes 的 etcd 使用 2379 端口，因此 Dapr Scheduler 默认也监听 2379，导致冲突。

### ✅ 解决方案
关闭 Docker Desktop 的 Kubernetes（推荐）：
```
Docker Desktop → Settings → Kubernetes → Disable Kubernetes
```

或改用自定义端口运行 Dapr Scheduler：
```powershell
docker run -d --name dapr_scheduler -p 2390:2379 daprio/dapr:1.16.2 ./scheduler --port 2390
```

---

## ⚙️ 三、问题二：`mkdir data: permission denied`

### 📍 现象
日志输出：
```
failed to create data dir: mkdir data: permission denied
```

### 🧩 原因
Dapr Scheduler 默认在容器内 `/data` 目录创建存储文件，但该镜像中 `/data` 没有写权限。

### ✅ 解决方案
挂载可写卷（Windows 路径示例）：
```powershell
mkdir C:\dapr\scheduler\data

docker run -d --name dapr_scheduler --restart always `
  -v "C:\dapr\scheduler\data:/data" `
  -p 2390:2379 -p 6060:50006 -p 58081:8080 -p 59091:9090 `
  daprio/dapr:1.16.2 ./scheduler --port 2390
```

---

## ⚙️ 四、问题三：桥接网络无法删除

执行：
```powershell
docker network rm bridge
```
报错：
```
Error response from daemon: bridge is a pre-defined network and cannot be removed
```

### 🧩 原因
`bridge` 是 Docker 默认网络，无法删除。

### ✅ 正确做法
自定义一个网络供 Dapr 服务互联：
```powershell
docker network create daprnet
```
然后在启动每个容器时添加：
```powershell
--network daprnet
```

---

## ⚙️ 五、完整启动命令汇总

```powershell
# 创建网络与数据卷
docker network create daprnet
mkdir C:\dapr\scheduler\data

# Redis
docker run -d --name dapr_redis --network daprnet redis

# Zipkin
docker run -d --name dapr_zipkin --network daprnet -p 9411:9411 openzipkin/zipkin

# Placement 服务
docker run -d --name dapr_placement --network daprnet -p 50005:50005 daprio/dapr:1.16.2 ./placement

# Scheduler（自定义端口与挂载目录）
docker run -d --name dapr_scheduler --network daprnet --restart always `
  -v "C:\dapr\scheduler\data:/data" `
  -p 2390:2379 -p 6060:50006 -p 58081:8080 -p 59091:9090 `
  daprio/dapr:1.16.2 ./scheduler --port 2390
```

---

## 🔍 六、验证与排查命令

### 查看容器状态
```powershell
docker ps --format "table {{.Names}}\t{{.Ports}}"
```

### 查看 Scheduler 日志
```powershell
docker logs dapr_scheduler --tail 30
```

### 确认端口监听
```powershell
netstat -ano | findstr "2390"
```

---

## 🧩 七、总结经验

| 问题 | 根因 | 解决方案 |
|------|------|-----------|
| 2379 端口被占用 | Docker Desktop 的 Kubernetes 内置 etcd | 改端口或禁用 Kubernetes |
| permission denied | 镜像内 `/data` 无写权限 | 使用 `-v` 挂载宿主机目录 |
| bridge 网络删除错误 | 系统预定义网络 | 创建自定义网络 daprnet |
| 无法修改端口 | CLI 无端口参数 | 手动 docker run 并加 `--port` |

---

## 🧱 八、扩展建议

- 推荐在 **Docker Compose** 中管理 Dapr 组件，方便配置持久化和端口。
- 若未来迁移至 **Kubernetes 模式**，Dapr 的 Scheduler、Placement、Sentry 会由 Helm 自动创建。
- 在 .NET 环境中使用 Dapr Sidecar 时，可在 `dapr.yaml` 中设置 Scheduler 地址为 `localhost:2390`。

---

## ✅ 九、最终状态
```text
✔ dapr_redis        -> redis:6379
✔ dapr_zipkin       -> 9411
✔ dapr_placement    -> 50005
✔ dapr_scheduler    -> 2390(gRPC) / 58081(http) / 59091(metrics)
```

一切运行正常。🎉
